using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSystem : MonoBehaviour
{
    #region Public Variables
    [field: Header("Movement")]
    [field: Tooltip("The speed that the player moves when on the ground.")]
    [field: SerializeField] private float MoveSpeed;

    [field: Tooltip("The speed that the player moves when scurrying on the ground.")]
    [field: SerializeField] private float ScurrySpeed;

    [field: Tooltip("The amount of time (in seconds) the player can scurry for before stopping")]
    [field: SerializeField] private float ScurryLimit;

    [field: Tooltip("The amount of time (in seconds) it takes for the player to recover their scurry ability")]
    [field: SerializeField] private float ScurryRecoveryTime;

    [field: Tooltip("The acceleration that will be applied to the player when they begin moving. Likewise, the time it takes for them to stop moving.")]
    [field: SerializeField] private float MoveEasing;

    [field: Tooltip("Locks the player's movement to a specific axis.")]
    [field: SerializeField] private MoveType MoveType = MoveType.TwoDimensionsOnly;

    [HideInInspector] public bool ClimbingRequested;
    [HideInInspector] public bool IsClimbing;
    [HideInInspector] public bool IsJumpingFromClimb;
    [HideInInspector] public bool FallingFromClimb;

    [field: Header("Jumping & Gravity")]
    [field: Tooltip("The force that is applied to the player's y-axis upon hitting the jump key/button.")]
    [field: SerializeField] private float JumpForce;

    [field: Tooltip("How much the gravity applied to the player is multiplied.")]
    [field: SerializeField] private float GravityMultiplier;
    [field: SerializeField] private float VelocityYIdle = 0.0f;

    [field: Header("Lerping")]
    [field: SerializeField] private bool LerpRotation;
    [field: SerializeField] private float LerpSpeed;

    [field: Tooltip("The force that the player will push objects.")]
    [field: SerializeField] private float PushForce;

    [field: Header("Animations")]
    [field: Tooltip("The Value Name that's targeted within the \"Animator\" reference")]
    [field: SerializeField] private PlayerAnimation[] PlayerAnimations;

    [field: Header("Externals")]
    [field: Tooltip("Reference to the camera that will follow the player.")]
    public CameraSystem Camera;
    public Animator Animator;

    [HideInInspector] public CharacterController Character;
    [HideInInspector] public bool IsHidden = false;

    [field: Header("Miscellaneous")]
    [field: SerializeField] private string SurfaceMaterialsPath = "SurfaceMaterials";
    [field: SerializeField] private SurfaceMaterial GenericSurface;
    [field: Tooltip("All the Surface Types that the Player can interact with")]
    private Dictionary<string, SurfaceMaterial> Surfaces = new();

    [field: Header("Events")]
    public PlayerEvents Events = new();
    #endregion

    #region Private Variables
    private Vector3 WarpPosition;
    private Quaternion WarpRotation;
    private Quaternion CharacterRotation;

    private Vector3 Velocity;
    private Vector2 MoveInput;
    private Vector3 HitDirection;
    private Vector3 LastFrameVelocity;
    private Vector3 MoveDelta;

    private CameraTarget OriginalSpawn;
    private SurfaceMaterial FloorMaterial;

    private float CurrentScurryTime;
    private float CurrentRecoveryTime;
    private float TimeUntilJumpButtonIsDisabled;

    private float CurrentMoveSpeed;

    private bool CanScurry = true;
    private bool JumpButtonIsHeld = false;

    private bool IsJumping, IsScurrying, IsGrounded, IsMoving;
    #endregion

    #region Functions - Handlers
    // Surely there's an easier way to format all of this... right?
    public void OnMove(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
        Events.Moving.Invoke(MoveInput);
    }
    public void OnClimbing(InputAction.CallbackContext ctx)
    {
        if (MoveType == MoveType.None) return;
        ClimbingRequested = ctx.ReadValueAsButton();
        Events.Climbing.Invoke(ClimbingRequested);
    }
    public void OnScurry(InputAction.CallbackContext ctx)
    {
        if (MoveType == MoveType.None || !ctx.ReadValueAsButton() || ctx.phase != InputActionPhase.Performed) return;
        IsScurrying = !IsScurrying;

        Events.Scurrying.Invoke(IsScurrying);

        if (IsScurrying || !CanScurry) return;
        CanScurry = false;
    }
    public void OnJumping(InputAction.CallbackContext ctx)
    {
        // Prevent holding the button from continuously firing inputs. Only fire once.
        if (MoveType == MoveType.None) return;
        IsJumping = ctx.ReadValueAsButton();
        Events.Jumping.Invoke(IsJumping);
    }
    public void OnInteracting(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Canceled || ctx.phase == InputActionPhase.Disabled) return;
        Events.Interacting.Invoke(ctx.ReadValueAsButton());
    }
    #endregion

    #region Functions - Public
    public Vector2 GetMoveInput() => MoveInput;
    public bool IsPlayerMoving() => IsMoving;
    public bool IsPlayerJumping() => IsJumping;
    public bool IsPlayerGrounded() => IsGrounded;
    public void Warp(Vector3 NewPosition) => WarpPosition = NewPosition;
    public void Warp(Vector3 NewPosition, Quaternion NewRotation)
    {
        WarpPosition = NewPosition;
        WarpRotation = NewRotation;
    }
    public void SetVelocity(Vector3 NewVelocity) => Velocity = NewVelocity;
    public MoveType GetMoveType() => MoveType;
    public void SetMoveType(MoveType Type, bool ResetVelocity = false)
    {
        MoveType = Type;
        if (!ResetVelocity) return;
        SetVelocity(Vector3.zero);
    }
    public void DeathTriggered()
    {
        /*
         * TODO:
         * 1. Trigger cut to black
         * 2. Hold screen for a few seconds
         * 3. Reload previously interacted objects and Scene data so they can be used again
         * 4. Fade from black with the player reloaded at the previous checkpoint
         * 5. Add to the current save file's "Deaths" data
         * 6. Save to disk!
         */

        SaveData data = DataHandler.Instance.GetCachedData();
        data.deaths++;

        DataHandler.Instance.SetCachedData(data);
        bool result = DataHandler.Instance.SaveCachedDataToFile();

        if (result) Debug.Log(name + " Successfully saved: " + DataHandler.Instance.GetFileName() + " to disk!");
        else Debug.LogWarning(name + " Failed to save: " + DataHandler.Instance.GetFileName() + " to disk... :(");

        SpawnAtCheckpoint();
    }
    #endregion

    #region Functions - Private
    private void SpawnAtCheckpoint()
    {
        SaveData data = DataHandler.Instance.RefreshCachedData();

        if (string.IsNullOrWhiteSpace(data.checkpointName))
        {
            Warp(OriginalSpawn.position, OriginalSpawn.rotation);
            return;
        }

        Vector3 Position = DataHandler.Instance.ConvertFloatArrayToVector3(data.checkpointPosition);
        Vector3 Eular = DataHandler.Instance.ConvertFloatArrayToVector3(data.checkpointRotation);
        Quaternion Rotation = Quaternion.Euler(Eular);

        Warp(Position, Rotation);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        if (body == null || body.isKinematic) return;
        if (hit.moveDirection.y < -0.3f) return;

        Vector3 pushDir = new(hit.moveDirection.x, 0, hit.moveDirection.z);
        Vector3 collisionPoint = hit.point;

        body.AddForceAtPosition(pushDir * PushForce, collisionPoint, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        if (IsGrounded)
        {
            PhysicMaterial physical = null;

            if (Physics.Raycast(new Ray(transform.position, Vector3.down), out RaycastHit hit)) physical = hit.collider.sharedMaterial;
            if (physical != null) Surfaces.TryGetValue(physical.name, out FloorMaterial);
            else FloorMaterial = GenericSurface;
        }
        else FloorMaterial = null;

        if (FloorMaterial != null)
        {
            if (FloorMaterial.PreventScurrying)
            {
                CurrentMoveSpeed = MoveSpeed;
                CanScurry = false;
            }

            switch (FloorMaterial.MathUsage)
            {
                case MathType.Addition: CurrentMoveSpeed += FloorMaterial.PlayerSpeedModifier; break;
                case MathType.Subtraction: CurrentMoveSpeed -= FloorMaterial.PlayerSpeedModifier; break;
                case MathType.Multiplication: CurrentMoveSpeed *= FloorMaterial.PlayerSpeedModifier; break;
                case MathType.Division: CurrentMoveSpeed /= FloorMaterial.PlayerSpeedModifier; break;
            }
        }

        CurrentMoveSpeed = (!IsScurrying && !IsJumpingFromClimb && !IsClimbing) ? MoveSpeed : ScurrySpeed;
        MoveDelta = (MoveInput.x * Camera.main.transform.right + MoveInput.y * Camera.main.transform.forward) * CurrentMoveSpeed;

        if (IsJumping)
        {
            // If IsJumping is true, set JumpButtonIsHeld to true after .05 seconds.
            TimeUntilJumpButtonIsDisabled += Time.fixedDeltaTime;
            if (TimeUntilJumpButtonIsDisabled > 0.05f) JumpButtonIsHeld = true;
        }

        if (IsClimbing)
        {
            if (!IsJumping && JumpButtonIsHeld) 
            { 
                JumpButtonIsHeld = false; 
                TimeUntilJumpButtonIsDisabled = 0; 
            }

            if (IsJumping && !JumpButtonIsHeld)
            {
                Velocity.y = JumpForce / 2.0f;
                IsClimbing = false;
                IsJumpingFromClimb = true;
            }
            else if (ClimbingRequested)
            {
                IsClimbing = false;
                FallingFromClimb = true;
            }
        }

        Vector3 actualVelocity;

        if (!IsClimbing)
        {
            Velocity.z = MoveDelta.z;
            Velocity.x = MoveDelta.x;

            if (IsGrounded)
            {
                IsJumpingFromClimb = false;
                FallingFromClimb = false;

                if (!IsJumping && JumpButtonIsHeld) { JumpButtonIsHeld = false; TimeUntilJumpButtonIsDisabled = 0; }

                if (IsJumping && !FloorMaterial.PreventJumping && !JumpButtonIsHeld) Velocity.y = JumpForce;
                else if (Velocity.y < VelocityYIdle) Velocity.y = VelocityYIdle;
            }

            Velocity += GravityMultiplier * Time.fixedDeltaTime * Physics.gravity;
            actualVelocity = Vector3.Lerp(LastFrameVelocity, Velocity, MoveEasing * Time.fixedDeltaTime);
        }
        else
        {
            // Properly lerp the movement up and down since it's really jarring moving linearly between movements

            Velocity.y = MoveDelta.z;

            actualVelocity = Vector3.Lerp(LastFrameVelocity, Velocity, MoveEasing * Time.fixedDeltaTime);
            actualVelocity.z = 0;
        }

        Character.Move(actualVelocity * Time.fixedDeltaTime);
        LastFrameVelocity = (!IsClimbing) ? new(actualVelocity.x, Velocity.y, actualVelocity.z) : new(actualVelocity.x, actualVelocity.y, Velocity.z);

        if (!IsGrounded) HitDirection = Vector3.zero;

        if (!IsMoving)
        {
            Vector3 horizonalHitDirection = HitDirection;
            horizonalHitDirection.y = 0;

            float displacement = horizonalHitDirection.magnitude;
            if (displacement <= 0) return;

            Velocity -= 0.2f * horizonalHitDirection / displacement;

            return;
        }

        if (IsClimbing)
        {
            // Climbing based rotation goes here!
            return;
        }

        float radian = Mathf.Atan2(MoveInput.y, MoveInput.x * -1.0f);
        float degree = 180.0f * radian / Mathf.PI;
        float rotation = (360.0f + Mathf.Round(degree)) % 360.0f;

        CharacterRotation = Quaternion.Euler(0.0f, IsMoving ? rotation + 90.0f : 90.0f, 0.0f);
        if (LerpRotation) CharacterRotation = Quaternion.Lerp(Character.transform.rotation, CharacterRotation, Time.fixedDeltaTime * LerpSpeed);

        Character.transform.rotation = CharacterRotation;
    }

    private void Update()
    {
        switch (MoveType)
        {
            case MoveType.None: MoveInput = Vector2.zero; break;
            case MoveType.LockToLeftRight: MoveInput.y = 0.0f; break;
            case MoveType.LockToForwardBack: MoveInput.x = 0.0f; break;
            case MoveType.TwoDimensionsOnly: MoveInput.y = (!IsClimbing) ? 0.0f : MoveInput.y; break;
        }

        IsGrounded = Character.isGrounded;
        IsMoving = MoveDelta.magnitude != 0.0f;

        if (Animator != null)
        {
            foreach (PlayerAnimation PA in PlayerAnimations)
            {
                if (PA.AnimationType == AnimType.Custom)
                {
                    object value = null;

                    switch (PA.AnimationValueType)
                    {
                        case AnimValueType.Integer: value = Animator.GetInteger(PA.ValueName); break;
                        case AnimValueType.Float: value = Animator.GetFloat(PA.ValueName); break;
                        case AnimValueType.Boolean: value = Animator.GetBool(PA.ValueName); break;
                    }

                    if (value == null) continue;
                }

                switch (PA.AnimationType)
                {
                    case AnimType.Custom:
                        {
                            switch (PA.AnimationValueType)
                            {
                                case AnimValueType.Integer:
                                    {
                                        bool result = int.TryParse(PA.InputValue, out int input);
                                        if (!result) continue;
                                        Animator.SetInteger(PA.ValueName, input);
                                    }
                                    break;

                                case AnimValueType.Float:
                                    {
                                        bool result = float.TryParse(PA.InputValue, out float input);
                                        if (!result) continue;
                                        Animator.SetFloat(PA.ValueName, input);
                                    }
                                    break;
                                case AnimValueType.Boolean:
                                    {
                                        bool result = bool.TryParse(PA.InputValue, out bool input);
                                        if (!result) continue;
                                        Animator.SetBool(PA.ValueName, input);
                                    }
                                    break;
                            }
                        }
                        break;

                    case AnimType.Moving: Animator.SetFloat(PA.ValueName, (IsGrounded && IsMoving) ? CurrentMoveSpeed : 0.0f); break;
                    case AnimType.Jumping: /* TODO: ADD CONTENT HERE! */ break;
                }
            }
        }

        if (IsScurrying)
        {
            if (CanScurry && CurrentScurryTime < ScurryLimit)
            {
                CurrentScurryTime += Time.deltaTime;
                return;
            }

            CanScurry = false;
            IsScurrying = false;

            return;
        }

        if (CanScurry) return;

        if (CurrentRecoveryTime < ScurryRecoveryTime)
        {
            CurrentRecoveryTime += Time.deltaTime;
            return;
        }

        CanScurry = true;

        CurrentScurryTime = 0.0f;
        CurrentRecoveryTime = 0.0f;
    }

    private void LateUpdate()
    {
        if (WarpPosition == Vector3.zero) return;

        Character.enabled = false;
        transform.position = WarpPosition;

        if (WarpRotation.eulerAngles != Vector3.zero)
        {
            transform.rotation = WarpRotation;
            CharacterRotation = WarpRotation;
        }

        Character.enabled = true;
        WarpRotation.eulerAngles = Vector3.zero;
        WarpPosition = Vector3.zero;
    }

    private void Start()
    {
        foreach (SurfaceMaterial material in Resources.LoadAll<SurfaceMaterial>(SurfaceMaterialsPath))
        {
            Surfaces.Add(material.Material.name, material);
        }

        OriginalSpawn = new()
        {
            position = transform.position,
            rotation = transform.rotation
        };

        SpawnAtCheckpoint();
    }

    private void Awake()
    {
        Character = GetComponent<CharacterController>();

        Events.Moving ??= new();
        Events.Jumping ??= new();
        Events.Scurrying ??= new();
        Events.Climbing ??= new();
        Events.Interacting ??= new();
    }
    #endregion
}
