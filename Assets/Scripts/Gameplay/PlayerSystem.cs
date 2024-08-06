using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    #region Public Variables
    [field: Header("Movement & Jumping")]

    [field: Tooltip("The speed that the player moves when on the ground.")]
    [field: SerializeField] float MoveSpeed;    // = 6
    
    [field: Tooltip("The force that is applied to the player's y-axis upon hitting the jump key/button.")]
    [field: SerializeField] float JumpForce;    // = 11.5
    
    [field: Tooltip("How much the gravity applied to the player is multiplied.")]
    [field: SerializeField] float GravityMultiplier;    // = 2.3
    [field: SerializeField] float VelocityYIdle = 0.0f; // = -4
    
    [field: Tooltip("Locks the player's movement to a specific axis.")]
    public MovementType MoveType = MovementType.FreeRoam;

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

    #region Functions - Updates & Awake
    private void FixedUpdate()
    {
        Vector3 moveDelta = (MoveInput.x * Camera.main.transform.right + MoveInput.y * Camera.main.transform.forward) * MoveSpeed;

        Velocity.z = moveDelta.z;
        Velocity.x = moveDelta.x;

        if (IsGrounded)
        {
            if (IsJumping) Velocity.y = JumpForce;
            else if (Velocity.y < VelocityYIdle) Velocity.y = VelocityYIdle;
        }

        Velocity += GravityMultiplier * Time.fixedDeltaTime * Physics.gravity;
        Character.Move(Velocity * Time.deltaTime);

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

        transform.position = WarpPosition;
        WarpPosition = Vector3.zero;
    }

    private void Awake() => Character = GetComponentInChildren<CharacterController>();
    #endregion
}
