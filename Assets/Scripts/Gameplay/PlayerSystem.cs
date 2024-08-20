using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSystem : MonoBehaviour
{
    #region Public Variables
    [field: Header("Movement")]
    [field: Tooltip("The speed that the player moves when on the ground.")]
    [field: SerializeField] float MoveSpeed;

    [field: Tooltip("The speed that the player moves when scurrying on the ground.")]
    [field: SerializeField] float ScurrySpeed;

    [field: Tooltip("The acceleration that will be applied to the player when they begin moving. Likewise, the time it takes for them to stop moving.")]
    [field: SerializeField] float MoveEasing;

    [field: Tooltip("Locks the player's movement to a specific axis.")]
    [field: SerializeField] private MovementType MoveType = MovementType.FreeRoam;
    [HideInInspector] public bool ClimbingRequested;
    [HideInInspector] public bool IsClimbing;

    [field: Header("Jumping & Gravity")]
    [field: Tooltip("The force that is applied to the player's y-axis upon hitting the jump key/button.")]
    [field: SerializeField] float JumpForce;
    
    [field: Tooltip("How much the gravity applied to the player is multiplied.")]
    [field: SerializeField] private float GravityMultiplier;
    [field: SerializeField] private float VelocityYIdle = 0.0f;

    [field: Header("Lerping")]
    [field: SerializeField] private bool LerpRotation;
    [field: SerializeField] private float LerpSpeed;

    [field: Tooltip("The force that the player will push objects.")]
    [field: SerializeField] float PushForce;

    [field: Header("External References")]
    [field: Tooltip("Reference to the camera that will follow the player.")]
    public CameraSystem Camera;
    [HideInInspector] public CharacterController Character;
    [HideInInspector] public bool IsHidden = false, IsOnWetCement = false;
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

    private bool IsJumping, IsScurrying, IsGrounded, IsMoving;
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
         * 3. Fade from black with the player reloaded at the previous checkpoint
         * 4. Add to the current save file's "Deaths" data
         * 5. Save to disk!
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

        print(Position.ToString());
        print(Rotation.ToString());

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
        Vector3 moveDelta = IsOnWetCement ?
            (MoveInput.x * Camera.main.transform.right + MoveInput.y * Camera.main.transform.forward) * (MoveSpeed / 1.85f): (IsScurrying ?
            (MoveInput.x * Camera.main.transform.right + MoveInput.y * Camera.main.transform.forward) * ScurrySpeed :
            (MoveInput.x * Camera.main.transform.right + MoveInput.y * Camera.main.transform.forward) * MoveSpeed);

        if (!IsClimbing)
        {
            Velocity.z = moveDelta.z;
            Velocity.x = moveDelta.x;

            if (IsGrounded)
            {
                if (IsJumping && !IsOnWetCement) Velocity.y = JumpForce;
                else if (Velocity.y < VelocityYIdle) Velocity.y = VelocityYIdle;
            }

            Velocity += GravityMultiplier * Time.fixedDeltaTime * Physics.gravity;
            Vector3 actualVelocity = Vector3.Lerp(lastFrameVelocity, Velocity, MoveEasing * Time.fixedDeltaTime);
            Character.Move(actualVelocity * Time.fixedDeltaTime);

            lastFrameVelocity = new(actualVelocity.x, Velocity.y, actualVelocity.z);
        }
        else
        {
            Velocity.y = moveDelta.z;

            Vector3 actualVelocity = Vector3.Lerp(lastFrameVelocity, Velocity, MoveEasing * Time.fixedDeltaTime);
            actualVelocity.z = 0;
            Character.Move(actualVelocity * Time.fixedDeltaTime);

            lastFrameVelocity = new(actualVelocity.x, actualVelocity.y, Velocity.z);
        }

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
