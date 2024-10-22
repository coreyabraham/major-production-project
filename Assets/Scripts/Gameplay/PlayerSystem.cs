using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSystem : MonoBehaviour
{
    #region Public Variables
    [field: Header("Movement")]
    [field: Tooltip("The speed that the player moves when on the ground.")]
    [field: SerializeField] private float MoveSpeed;

    [field: Tooltip("The speed that the player moves when climbing.")]
    [field: SerializeField] private float ClimbSpeed;

    [field: Tooltip("The speed that the player moves when scurrying on the ground.")]
    [field: SerializeField] private float ScurrySpeed;

    [field: Tooltip("The amount of time (in seconds) the player can scurry for before stopping")]
    [field: SerializeField] private float ScurryLimit;

    [field: Tooltip("The amount of time (in seconds) it takes for the player to recover their scurry ability")]
    [field: SerializeField] private float ScurryRecoveryTime;

    [field: Tooltip("The multiplier for how much the player's speed should be inhibited while pulling an object.\n\nNote that negative values will be flipped to be positive, should they be provided.")]
    [field: SerializeField] private float PullInhibitMultiplier;

    [field: Tooltip("The acceleration that will be applied to the player when they begin moving. Likewise, the time it takes for them to stop moving.")]
    [field: SerializeField] private float MoveEasing;

    [field: Tooltip("Locks the player's movement to a specific axis.")]
    [field: SerializeField] private MoveType MoveType = MoveType.TwoDimensionsOnly;

    [field: Tooltip("Unhook the player's movement from the camera.")]
    [field: SerializeField] private bool UnhookMovement = true;

    [field: Tooltip("Allows the player to jump for a short period of time after falling from a ledge.")]
    [field: SerializeField] private bool EnableCoyoteJump = true;
    
    [field: Tooltip("The duration of time that the player can fall for and coyote jump.")]
    [field: SerializeField] private float CoyoteTimer = 1.0f;

    [HideInInspector] public bool ClimbingRequested;
    [HideInInspector] public bool IsClimbing;
    [HideInInspector] public bool IsJumpingFromClimb;
    [HideInInspector] public bool FallingFromClimb;

    [field: Header("Jumping & Gravity")]
    [field: Tooltip("The force that is applied to the player's y-axis upon hitting the jump key/button.")]
    [field: SerializeField] private float JumpForce;

    [field: Tooltip("The force that us applied to the player's z-axis upon hitting the jump key/button when climbing on a pipe.")]
    [field: SerializeField] private float JumpForcePipe;

    [field: Tooltip("")]
    [field: SerializeField] private bool EnableScurryInAir;

    [field: Tooltip("How much the gravity applied to the player is multiplied.")]
    [field: SerializeField] private float GravityMultiplier;
    [field: SerializeField] private float VelocityYIdle = 0.0f;

    [field: Header("Lerping")]
    [field: SerializeField] private EasingStyle LerpStyle;
    [field: SerializeField] private float LerpSpeed;

    [field: Tooltip("The force that the player will push objects.")]
    [field: SerializeField] private float PushForce;

    [field: Header("Externals")]
    [field: Tooltip("Reference to the camera that will follow the player.")]
    public CameraSystem Camera;
    [field: Tooltip("Reference to the player model's Animator that's used to animate the character.")]
    public Animator Animator;

    [field: Header("Interacting")]
    [field: Tooltip("The GameObject Tag used to identify ALL Interactable GameObjects within the current scene.")]
    [field: SerializeField] private string InteractTag = "Interactable";
    [field: Tooltip("The GameObject Tag used to identify ALL Touchable GameObjects within the current scene.")]
    [field: SerializeField] private string TouchTag = "Touchable";

    [HideInInspector] public CharacterController Character;
    [HideInInspector] public bool IsHidden = false;

    [field: Header("Miscellaneous")]
    [field: SerializeField] private bool IgnoreCheckpointData = false;
    [field: SerializeField] private string SurfaceMaterialsPath = "SurfaceMaterials";
    [field: SerializeField] private SurfaceMaterial GenericSurface;
    [field: Tooltip("All the Surface Types that the Player can interact with")]
    private Dictionary<string, SurfaceMaterial> Surfaces = new();

    [field: Header("Events")]
    [field: SerializeField] public PlayerEvents Events = new();
    #endregion

    #region Private Variables
    [HideInInspector] public Vector3 WarpPosition;
    private Quaternion WarpRotation;
    private Quaternion CharacterRotation;

    private Vector2 MoveInput;
    private Vector3 MoveDelta;
    private Vector3 PullObjPos;
    private Vector3 HitDirection;

    private Vector3 Velocity;
    private Vector3 LastFrameVelocity = Vector3.zero;

    private CameraTarget OriginalSpawn;
    private SurfaceMaterial FloorMaterial;

    private bool UsedCoyoteJump;
    private float CurrentCoyoteTime;

    private float CurrentScurryTime;
    private float CurrentRecoveryTime;
    private float TimeUntilJumpButtonIsDisabled;

    private float SetMoveSpeed = 0.0f;

    private float CurrentMoveSpeed = 0.0f;
    private float PreviousMoveSpeed;

    [HideInInspector] public PipeFunctionality CurrentPipe;
    [HideInInspector] public PipeSide CurrentPipeSide;

    private bool CanClimbUp = true;

    private bool CanScurry = true;
    private bool JumpButtonIsHeld = false;


    [HideInInspector] public bool IsJumping;

    private bool IsScurrying, IsGrounded, IsMoving, IsPulling, IsBeingLaunched;

    private readonly List<GameObject> CachedInteractables = new();
    private readonly Dictionary<GameObject, ITouchable> CachedTouchables = new();
    #endregion

    #region Functions - Handlers
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
        if (MoveType == MoveType.None || !ctx.ReadValueAsButton() || ctx.phase != InputActionPhase.Performed ||
            IsClimbing || IsPulling || IsJumpingFromClimb) return;
        
        IsScurrying = !IsScurrying;

        Events.Scurrying.Invoke(IsScurrying);

        if (IsScurrying || !CanScurry || !IsMoving) return;
        CanScurry = false;
    }
    public void OnJumping(InputAction.CallbackContext ctx)
    {
        if (MoveType == MoveType.None) return;
        if ((IsClimbing && CurrentPipeSide == PipeSide.Left && MoveInput.x > 0) ||
            (IsClimbing && CurrentPipeSide == PipeSide.Right && MoveInput.x < 0) ||
            (IsClimbing && MoveInput.x == 0)) { return; }

        IsJumping = ctx.ReadValueAsButton();
        Events.Jumping.Invoke(IsJumping);
    }
    public void OnInteracting(InputAction.CallbackContext ctx)
    {
        if (ctx.phase != InputActionPhase.Performed) return;

        bool interactResult = ctx.ReadValueAsButton();
        Events.Interacting.Invoke(interactResult);

        if (!interactResult) return;

        // TODO: Is this possible to optimize? I don't know how to feel about using .GetComponent() more than necessary!
        foreach (GameObject interactable in CachedInteractables)
        {
            interactable.GetComponent<IInteractable>().Interact(interactable);
        }
    }
    #endregion

    #region Functions - Public
    public Vector2 GetMoveInput() => MoveInput;
    public bool IsPlayerMoving() => IsMoving;
    public bool IsPlayerJumping() => IsJumping;
    public bool IsPlayerGrounded() => IsGrounded;
    public bool ToggleUpMovement(bool enable) => CanClimbUp = enable;
    public bool TogglePullState(bool input) => IsPulling = input;
    public bool ToggleCharCont(bool enable) => Character.enabled = enable;
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
    public void ForcePlayerToJump(float forceToApply) => Velocity.y = forceToApply;
    public void ApplyImpulseToPlayer(float accuracy)
    {
        IsBeingLaunched = true;
        Velocity.x = 7 * accuracy;
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

        GameSystem.Instance.PlayerDiedCallback();
        SpawnAtCheckpoint();
    }
    #endregion

    #region Functions - Private
    private void SpawnAtCheckpoint()
    {
        SaveData data = DataHandler.Instance.RefreshCachedData();

        if (!GameSystem.Instance.RunningInEditor && IgnoreCheckpointData == true) IgnoreCheckpointData = false;

        if (string.IsNullOrWhiteSpace(data.checkpointName) || IgnoreCheckpointData == true)
        {
            Warp(OriginalSpawn.position, OriginalSpawn.rotation);
            return;
        }

        Vector3 Position = DataHandler.Instance.ConvertFloatArrayToVector3(data.checkpointPosition);
        Vector3 Eular = DataHandler.Instance.ConvertFloatArrayToVector3(data.checkpointRotation);
        Quaternion Rotation = Quaternion.Euler(Eular);

        Warp(Position, Rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(TouchTag) != true) return;

        bool result = CachedTouchables.TryGetValue(other.gameObject, out ITouchable touchable);
        if (!result) return;

        touchable.TriggerEnter(this);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(TouchTag))
        {
            if (CachedTouchables.TryGetValue(other.gameObject, out ITouchable touchable))
                touchable.TriggerStay(this);
        }

        if (!other.transform.root.CompareTag("Grabbable")) { return; }
        if (!TogglePullState(Input.GetKey(KeyCode.E))) { return; }

        PullObjPos = other.transform.root.transform.position;
        float grabX;

        // I know this is an atrocious way of checking which side of the object the player is on...
        if (transform.position.x < other.transform.root.transform.position.x)
        {
            // On the left of the object.
            grabX = transform.position.x - 0.1f;
        }
        else
        {
            // On the right of the object.
            grabX = transform.position.x + 0.1f;
        }

        other.transform.root.transform.position = new(grabX - other.transform.localPosition.x, other.transform.root.transform.position.y, other.transform.root.transform.position.z);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag("Grabbable")) { PullObjPos = Vector3.zero; }
        if (other.gameObject.CompareTag(TouchTag) != true) return;

        bool result = CachedTouchables.TryGetValue(other.gameObject, out ITouchable touchable);
        if (!result) return;

        touchable.TriggerLeave(this);
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
                SetMoveSpeed = MoveSpeed;
                CanScurry = false;
            }

            switch (FloorMaterial.MathUsage)
            {
                case MathType.Addition: SetMoveSpeed += FloorMaterial.PlayerSpeedModifier; break;
                case MathType.Subtraction: SetMoveSpeed -= FloorMaterial.PlayerSpeedModifier; break;
                case MathType.Multiplication: SetMoveSpeed *= FloorMaterial.PlayerSpeedModifier; break;
                case MathType.Division: SetMoveSpeed /= FloorMaterial.PlayerSpeedModifier; break;
            }
        }

        //SetMoveSpeed = (!IsScurrying && !IsJumpingFromClimb && !IsClimbing) ? MoveSpeed : ScurrySpeed;
        if ((!IsJumpingFromClimb && !IsScurrying && !IsClimbing)) { SetMoveSpeed = MoveSpeed; }
        else if (IsJumpingFromClimb) { SetMoveSpeed = JumpForcePipe; }
        else if (!IsJumpingFromClimb && !IsScurrying && IsClimbing) { SetMoveSpeed = ClimbSpeed; }
        else { SetMoveSpeed = ScurrySpeed; }

        Vector3 right = (!UnhookMovement) ? Camera.main.transform.right : Vector3.right;
        Vector3 forward = (!UnhookMovement) ? Camera.main.transform.forward : Vector3.forward;

        if (!CanClimbUp && MoveInput.y > 0) { MoveInput.y = 0; }

        MoveDelta = (MoveInput.x * right + MoveInput.y * forward) * SetMoveSpeed;

        if (IsJumping)
        {
            // If IsJumping is true, set JumpButtonIsHeld to true after .05 seconds.
            TimeUntilJumpButtonIsDisabled += Time.fixedDeltaTime;
            if (TimeUntilJumpButtonIsDisabled > 0.05f) JumpButtonIsHeld = true;
        }

        if (IsClimbing)
        {
            if (IsScurrying) { IsScurrying = false; }

            if (!IsJumping && JumpButtonIsHeld)
            {
                JumpButtonIsHeld = false;
                TimeUntilJumpButtonIsDisabled = 0;
            }

            if (IsJumping && !JumpButtonIsHeld)
            {
                if (IsJumpingFromClimb && CurrentPipeSide == PipeSide.Right) { MoveInput.x = 1; }
                else if (IsJumpingFromClimb && CurrentPipeSide == PipeSide.Left) { MoveInput.x = -1; }

                Velocity.y = JumpForce;
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
            if (!IsBeingLaunched)
            {
                Velocity.z = MoveDelta.z;
                Velocity.x = MoveDelta.x;
            }

            if (IsGrounded)
            {
                IsJumpingFromClimb = false;
                FallingFromClimb = false;
                IsBeingLaunched = false;

                if (!IsJumping && JumpButtonIsHeld) { JumpButtonIsHeld = false; TimeUntilJumpButtonIsDisabled = 0; }

                if (IsJumping && !FloorMaterial.PreventJumping && !JumpButtonIsHeld) Velocity.y = JumpForce;
                else if (Velocity.y < VelocityYIdle) Velocity.y = VelocityYIdle;
            }

            else
            {
                if (!EnableScurryInAir) { CanScurry = false; }

                if (EnableCoyoteJump && !UsedCoyoteJump && CurrentCoyoteTime < CoyoteTimer)
                {
                    CurrentCoyoteTime += Time.fixedDeltaTime;

                    if (IsJumping)
                    {
                        Velocity.y = JumpForce;
                        UsedCoyoteJump = true;
                    }
                }
            }

            Velocity += GravityMultiplier * Time.fixedDeltaTime * Physics.gravity;
            actualVelocity = Vector3.Lerp(LastFrameVelocity, Velocity, MoveEasing * Time.fixedDeltaTime);
        }
        else
        {
            Velocity.y = MoveDelta.z;

            actualVelocity = Vector3.Lerp(LastFrameVelocity, Velocity, 100 * Time.fixedDeltaTime);
            actualVelocity.z = 0;
        }

        PreviousMoveSpeed = CurrentMoveSpeed;
        CurrentMoveSpeed = new Vector2(actualVelocity.x, actualVelocity.z).magnitude;

        if (CurrentMoveSpeed < 0.0f) CurrentMoveSpeed = 0.0f;

        if (!IsPulling) Character.Move(actualVelocity * Time.fixedDeltaTime);
        else Character.Move((actualVelocity / PullInhibitMultiplier) * Time.fixedDeltaTime);

        LastFrameVelocity = (!IsClimbing) ? new(actualVelocity.x, Velocity.y, actualVelocity.z) : new(actualVelocity.x, actualVelocity.y, Velocity.z);

        if (!IsGrounded) HitDirection = Vector3.zero;
        else { UsedCoyoteJump = false; CurrentCoyoteTime = 0.0f; }

        if (!IsMoving)
        {
            Vector3 horizonalHitDirection = HitDirection;
            horizonalHitDirection.y = 0;

            float displacement = horizonalHitDirection.magnitude;
            if (displacement <= 0) return;

            Velocity -= 0.2f * horizonalHitDirection / displacement;

            return;
        }

        float radian = Mathf.Atan2(MoveInput.y, MoveDelta.x * -1.0f);
        float degree = 180.0f * radian / Mathf.PI;
        float rotation = (360.0f + Mathf.Round(degree)) % 360.0f;

        if (!IsPulling)
        {
            CharacterRotation = Quaternion.Euler(
                !IsClimbing ? 0.0f : 90.0f,
                !IsClimbing ? rotation + 90.0f : 180.0f,
                0.0f // Original: IsClimbing ? 180.0f : 0.0f
            );
        }
        else
        {
            if (transform.position.x > PullObjPos.x) CharacterRotation = Quaternion.Euler(0, PullObjPos.y + 90, 0);
            else CharacterRotation = Quaternion.Euler(0, PullObjPos.y - 90, 0);
        }

        switch (LerpStyle)
        {
            case EasingStyle.Basic: CharacterRotation = Quaternion.RotateTowards(Character.transform.rotation, CharacterRotation, Time.fixedDeltaTime * LerpSpeed); break;
            case EasingStyle.Lerp: CharacterRotation = Quaternion.Lerp(Character.transform.rotation, CharacterRotation, Time.fixedDeltaTime * LerpSpeed); break;
            case EasingStyle.Slerp: CharacterRotation = Quaternion.Slerp(Character.transform.rotation, CharacterRotation, Time.fixedDeltaTime * LerpSpeed); break;
        }

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

        Animator.SetFloat("Speed", CurrentMoveSpeed);
        Animator.SetBool("Jump", IsJumping && !IsGrounded);
        Animator.SetBool("Climb", IsClimbing);

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

        GameObject[] InteractArray = GameObject.FindGameObjectsWithTag(InteractTag);
        GameObject[] TriggerArray = GameObject.FindGameObjectsWithTag(TouchTag);

        foreach (GameObject obj in InteractArray)
        {
            bool result = obj.TryGetComponent(out IInteractable _);
            if (!result) continue;
            CachedInteractables.Add(obj);
        }

        foreach (GameObject obj in TriggerArray)
        {
            bool result = obj.TryGetComponent(out ITouchable touchable);
            if (!result) continue;
            CachedTouchables.Add(obj, touchable);
        }

        if (PullInhibitMultiplier < 0) { PullInhibitMultiplier = -PullInhibitMultiplier; }

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
