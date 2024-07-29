using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    [field: SerializeField] private CameraSystem mainCamera;

    [field: SerializeField] private float MoveSpeed;
    [field: SerializeField] private float JumpHeight;

    [field: SerializeField] private float VelocityYIdle = 0.0f;

    [field: SerializeField] private MovementType MoveType = MovementType.FreeRoam;

    [HideInInspector] public CharacterController Character;

    private Vector3 Velocity;
    private Vector2 MoveInput;

    private bool IsJumping;
    private bool IsGrounded;
    private bool IsMoving;

    public void HandleJumping(bool JumpBool) => IsJumping = JumpBool;
    public void HandleMovement(Vector2 moveInput) => MoveInput = moveInput;

    private void FixedUpdate()
    {
        Vector3 moveDelta = (MoveInput.x * mainCamera.main.transform.right + MoveInput.y * mainCamera.main.transform.forward) * MoveSpeed;

        if (IsGrounded || IsMoving)
        {
            Velocity.x = moveDelta.x;
            Velocity.z = moveDelta.z;
        }

        if (IsGrounded)
        {
            if (IsJumping) Velocity.y = JumpHeight;
            else if (Velocity.y < VelocityYIdle) Velocity.y = VelocityYIdle;
        }

        Velocity += Physics.gravity * Time.fixedDeltaTime;
        Character.Move(Velocity * Time.deltaTime);
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

    private void Start() => Character = GetComponentInChildren<CharacterController>();
}
