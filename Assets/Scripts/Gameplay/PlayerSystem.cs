using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    #region Variables
    [Header("Movement & Jumping")]
    [Tooltip("The speed that the player moves when on the ground.")]
    [field: SerializeField] float MoveSpeed;    // = 6
    [Tooltip("The force that is applied to the player's y-axis upon hitting the jump key/button.")]
    [field: SerializeField] float JumpForce;    // = 11.5
    [Tooltip("How much the gravity applied to the player is multiplied.")]
    [field: SerializeField] float GravityMultiplier;    // = 2.3
    [field: SerializeField] float VelocityYIdle = 0.0f; // = -4
    [Tooltip("Locks the player's movement to a specific axis.")]
    public MovementType MoveType = MovementType.FreeRoam;
    Vector3 Velocity;
    Vector2 MoveInput;
    bool IsJumping, IsGrounded, IsMoving;

    [Header("External References")]
    [Tooltip("Reference to the camera that will follow the player.")]
    public CameraSystem Camera;
    [HideInInspector] public CharacterController Character;
    #endregion


    #region Functions - Handlers
    public void HandleJumping(bool JumpBool) => IsJumping = JumpBool;
    public void HandleMovement(Vector2 moveInput) => MoveInput = moveInput;
    #endregion


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
        Character.Move(Velocity * Time.deltaTime);
    }


    private void Awake() => Character = GetComponentInChildren<CharacterController>();
    #endregion
}
