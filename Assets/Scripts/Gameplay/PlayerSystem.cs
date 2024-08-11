using UnityEngine;


public class PlayerSystem : MonoBehaviour
{
    #region Public Variables
    [field: Header("Movement")]
    [field: Tooltip("The speed that the player moves when on the ground.")]
    [field: SerializeField] float MoveSpeed;    // = 6

    [field: Tooltip("The speed that the player moves when scurrying on the ground.")]
    [field: SerializeField] float ScurrySpeed;

    [field: Tooltip("The acceleration that will be applied to the player when they begin moving. Likewise, the time it takes for them to stop moving.")]
    [field: SerializeField] float MoveEasing;

    public MovementType MoveType = MovementType.FreeRoam;
    [HideInInspector] public bool isClimbing = false;

    [field: Header("Jumping & Gravity")]
    [field: Tooltip("The force that is applied to the player's y-axis upon hitting the jump key/button.")]
    [field: SerializeField] float JumpForce;
    
    [field: Tooltip("How much the gravity applied to the player is multiplied.")]
    [field: SerializeField] float GravityMultiplier;
    [field: SerializeField] float VelocityYIdle = 0.0f;
    
    [field: Tooltip("Locks the player's movement to a specific axis.")]

    [field: Header("Lerping")]
    [field: SerializeField] private bool LerpRotation;
    [field: SerializeField] private float LerpSpeed;

    [field: Header("External References")]
    [field: Tooltip("Reference to the camera that will follow the player.")]
    public CameraSystem Camera;
    [HideInInspector] public CharacterController Character;

    #endregion

    #region Private Variables
    private Vector3 WarpPosition;
    private Quaternion CharacterRotation;

    private Vector3 lastFrameVelocity = new(0, 0, 0);
    private Vector3 Velocity;
    private Vector2 MoveInput;
    private bool IsJumping, IsGrounded, IsMoving;
    #endregion

    #region Functions - Handlers
    public void HandleMovement(Vector2 moveInput) => MoveInput = moveInput;

    public void HandleJumping(bool JumpBool)
    {
        if (MoveType == MovementType.None) return;
        IsJumping = JumpBool;
    }
    #endregion

    public void WarpToPosition(Vector3 NewPosition) => WarpPosition = NewPosition;


    public void ToggleClimbingMode()
    {
        Velocity = new(0, 0, 0);

        if (!isClimbing)
        {
            // Enable Climbing

            isClimbing = true;
        }
        else
        {
            // Disable Climbing

            isClimbing = false;
        }
    }


    #region Functions - Updates & Awake
    private void FixedUpdate()
    {
        Vector3 moveDelta;
        
        if (Input.GetKey(KeyCode.LeftControl))
        {
            moveDelta = (MoveInput.x * Camera.main.transform.right + MoveInput.y * Camera.main.transform.forward) * ScurrySpeed;
        }
        else
        {
            moveDelta = (MoveInput.x * Camera.main.transform.right + MoveInput.y * Camera.main.transform.forward) * MoveSpeed;
        }


        if (!isClimbing)
        {
            Velocity.z = moveDelta.z;
            Velocity.x = moveDelta.x;

            if (IsGrounded)
            {
                if (IsJumping) Velocity.y = JumpForce;
                else if (Velocity.y < VelocityYIdle) Velocity.y = VelocityYIdle;
            }

            Velocity += GravityMultiplier * Time.fixedDeltaTime * Physics.gravity;
            Vector3 actualVelocity = Vector3.MoveTowards(lastFrameVelocity, Velocity, MoveEasing * Time.fixedDeltaTime);
            Character.Move(actualVelocity * Time.fixedDeltaTime);

            lastFrameVelocity = new(actualVelocity.x, Velocity.y, actualVelocity.z);
        }
        else
        {
            Velocity.y = moveDelta.z;

            Vector3 actualVelocity = Vector3.MoveTowards(lastFrameVelocity, Velocity, MoveEasing * Time.fixedDeltaTime);
            actualVelocity.z = 0;
            Character.Move(actualVelocity * Time.fixedDeltaTime);

            lastFrameVelocity = new(actualVelocity.x, actualVelocity.y, Velocity.z);
        }

        
        if (!IsMoving) return;

        float radian = Mathf.Atan2(MoveInput.y, MoveInput.x * -1.0f);
        float degree = 180.0f * radian / Mathf.PI;
        float rotation = (360.0f + Mathf.Round(degree)) % 360.0f;


        CharacterRotation = Quaternion.Euler(0.0f, IsMoving ? rotation + 90.0f : 90.0f, 0.0f);


        if (LerpRotation)
        {
            CharacterRotation = Quaternion.Lerp(Character.transform.rotation, CharacterRotation, Time.fixedDeltaTime * LerpSpeed);
        }

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
        Character.enabled = true;

        WarpPosition = Vector3.zero;
    }

    private void Awake() => Character = GetComponent<CharacterController>();
    #endregion
}
