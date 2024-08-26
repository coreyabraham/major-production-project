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

    [field: Tooltip("The acceleration that will be applied to the player when they begin moving. Likewise, the time it takes for them to stop moving.")]
    [field: SerializeField] private float MoveEasing;

    [field: Tooltip("Locks the player's movement to a specific axis.")]
    [field: SerializeField] private MovementType MoveType = MovementType.FreeRoam;

    [HideInInspector] public bool ClimbingRequested;
    [HideInInspector] public bool IsClimbing;
    [HideInInspector] public bool IsJumpingFromClimb;

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

    [field: Header("External References")]
    [field: Tooltip("Reference to the camera that will follow the player.")]
    public CameraSystem Camera;

    [HideInInspector] public CharacterController Character;
    [HideInInspector] public bool IsHidden = false;

    [field: Header("Miscellaneous")]
    [field: SerializeField] private string SurfaceMaterialsPath = "SurfaceMaterials";
    [field: SerializeField] private SurfaceMaterial GenericSurface;
    [field: Tooltip("All the Surface Types that the Player can interact with")]
    private Dictionary<string, SurfaceMaterial> Surfaces = new();
    #endregion

    #region Private Variables
    private Vector3 WarpPosition;
    private Quaternion WarpRotation;
    private Quaternion CharacterRotation;

    private Vector3 Velocity;
    private Vector2 MoveInput;
    private Vector3 HitDirection;

    private Vector3 lastFrameVelocity = Vector3.zero;

    private CameraTarget OriginalSpawn;
    private SurfaceMaterial FloorMaterial;

    private bool IsJumping, IsScurrying, IsGrounded, IsMoving, IsScurryingLocked;
    #endregion

    #region Functions - Handlers
    public void OnMove(InputAction.CallbackContext ctx) => MoveInput = ctx.ReadValue<Vector2>();
    public void OnScurry(InputAction.CallbackContext ctx) => IsScurrying = ctx.ReadValueAsButton();
    public void OnClimbing(InputAction.CallbackContext ctx) => ClimbingRequested = ctx.ReadValueAsButton();
    public void OnJumping(InputAction.CallbackContext ctx)
    {
        if (MoveType == MovementType.None) return;
        IsJumping = ctx.ReadValueAsButton();
    }
    #endregion

    #region Functions - Public
    public void Warp(Vector3 NewPosition) => WarpPosition = NewPosition;
    public void Warp(Vector3 NewPosition, Quaternion NewRotation)
    {
        WarpPosition = NewPosition;
        WarpRotation = NewRotation;
    }
    public void SetVelocity(Vector3 NewVelocity) => Velocity = NewVelocity;
    public MovementType GetMovementType() => MoveType;
    public void SetMovementType(MovementType Type, bool ResetVelocity = false)
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
        PhysicMaterial physical = null;

        if (Physics.Raycast(new Ray(transform.position, Vector3.down), out RaycastHit hit)) physical = hit.collider.sharedMaterial;
        if (physical != null) Surfaces.TryGetValue(physical.name, out FloorMaterial);
        else FloorMaterial = (IsGrounded) ? GenericSurface : null;

        float speed = (!IsScurrying && !IsScurryingLocked && !IsClimbing) ? MoveSpeed : ScurrySpeed;

        if (FloorMaterial != null)
        {
            if (FloorMaterial.PreventScurrying) speed = MoveSpeed;

            switch (FloorMaterial.MathUsage)
            {
                case MathType.Addition: speed += FloorMaterial.PlayerSpeedModifier; break;
                case MathType.Subtraction: speed -= FloorMaterial.PlayerSpeedModifier; break;
                case MathType.Multiplication: speed *= FloorMaterial.PlayerSpeedModifier; break;
                case MathType.Division: speed /= FloorMaterial.PlayerSpeedModifier; break;
            }
        }

        Vector3 moveDelta = (MoveInput.x * Camera.main.transform.right + MoveInput.y * Camera.main.transform.forward) * speed;

        if (IsClimbing)
        {
            if (IsJumping)
            {
                IsClimbing = false;
                Velocity.y = JumpForce / 2.0f;

                IsScurryingLocked = true;
                IsJumpingFromClimb = true;
            }
            else if (ClimbingRequested) IsClimbing = false;
        }

        Vector3 actualVelocity;

        if (!IsClimbing)
        {
            Velocity.z = moveDelta.z;
            Velocity.x = moveDelta.x;

            if (IsGrounded)
            {
                IsScurryingLocked = false;
                IsJumpingFromClimb = false;

                if (IsJumping && !FloorMaterial.PreventJumping) Velocity.y = JumpForce;
                else if (Velocity.y < VelocityYIdle) Velocity.y = VelocityYIdle;
            }

            Velocity += GravityMultiplier * Time.fixedDeltaTime * Physics.gravity;
            actualVelocity = Vector3.Lerp(lastFrameVelocity, Velocity, MoveEasing * Time.fixedDeltaTime);
        }
        else
        {
            // Properly lerp the movement up and down since it's really jarring moving linearly between movements

            Velocity.y = moveDelta.z;

            actualVelocity = Vector3.Lerp(lastFrameVelocity, Velocity, MoveEasing * Time.fixedDeltaTime);
            actualVelocity.z = 0;
        }

        Character.Move(actualVelocity * Time.fixedDeltaTime);
        lastFrameVelocity = (!IsClimbing) ? new(actualVelocity.x, Velocity.y, actualVelocity.z) : new(actualVelocity.x, actualVelocity.y, Velocity.z);

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
        IsGrounded = Character.isGrounded;

        switch (MoveType)
        {
            case MovementType.None: MoveInput = Vector2.zero; break;
            case MovementType.LockToLeftRight: MoveInput.y = 0.0f; break;
            case MovementType.LockToForwardBack: MoveInput.x = 0.0f; break;
        }

        IsMoving = MoveInput.x != 0 || MoveInput.y != 0;


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

    private void Awake() => Character = GetComponent<CharacterController>();
    #endregion
}
