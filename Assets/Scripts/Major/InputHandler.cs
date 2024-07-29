using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [System.Serializable]
    public class InputEvents
    {
        public UnityEvent<Vector2> Moving;
        public UnityEvent<bool> Jumping;

        public UnityEvent MoveCamLeft;
        public UnityEvent MoveCamRight;
    }

    [field: Header("Settings")]
    public bool CanUseInputs = true;

    [field: Space(2.5f)]

    [field: SerializeField] private InputEvents Events;

    private Actions Inputs;

    public void PlayerMoved(InputAction.CallbackContext ctx)
    {
        if (!CanUseInputs) return;
        Events.Moving?.Invoke(ctx.ReadValue<Vector2>());
    }

    public void PlayerJumping(InputAction.CallbackContext ctx)
    {
        if (!CanUseInputs) return;
        Events.Jumping?.Invoke(ctx.ReadValueAsButton());
    }

    public void CameraMoveLeft(InputAction.CallbackContext ctx)
    {
        if (!CanUseInputs) return;
        Events.MoveCamLeft?.Invoke();
    }

    public void CameraMoveRight(InputAction.CallbackContext ctx)
    {
        if (!CanUseInputs) return;
        Events.MoveCamRight?.Invoke();
    }

    private void OnEnable()
    {
        Inputs.Player.Move.performed += PlayerMoved;
        Inputs.Player.Move.started += PlayerMoved;
        Inputs.Player.Move.canceled += PlayerMoved;

        Inputs.Player.Jump.performed += PlayerJumping;
        Inputs.Player.Jump.canceled += PlayerJumping;

        Inputs.Player.MoveCamLeft.performed += CameraMoveLeft;
        Inputs.Player.MoveCamRight.performed += CameraMoveRight;

        Inputs.Player.Enable();
    }

    private void OnDisable()
    {
        Inputs.Player.Move.performed -= PlayerMoved;
        Inputs.Player.Move.started -= PlayerMoved;
        Inputs.Player.Move.canceled -= PlayerMoved;

        Inputs.Player.Jump.performed -= PlayerJumping;
        Inputs.Player.Jump.canceled -= PlayerJumping;

        Inputs.Player.MoveCamLeft.performed -= CameraMoveLeft;
        Inputs.Player.MoveCamRight.performed -= CameraMoveRight;

        Inputs.Player.Disable();
    }

    private void Awake() => Inputs = new();
}
