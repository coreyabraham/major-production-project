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
        public UnityEvent<bool> Climbing;
        public UnityEvent<bool> Scurrying;
    }

    [field: Header("Settings")]
    public bool CanUseInputs = true;

    [field: Space(2.5f)]

    [field: SerializeField] private InputEvents Events;

    private Actions Inputs;

    //public void InputCalled<TValue>(InputAction.CallbackContext ctx, EventData target, bool readAsButton = false)
    //{
    //    if (!CanUseInputs) return;
    //    target.Value = readAsButton ? ctx.ReadValueAsButton() : ctx.ReadValue<TValue>();
    //    target.Event?.Invoke(target.Value);
    //}

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

    public void PlayerClimbing(InputAction.CallbackContext ctx)
    {
        if (!CanUseInputs) return;
        Events.Climbing?.Invoke(ctx.ReadValueAsButton());
    }

    public void PlayerScurrying(InputAction.CallbackContext ctx)
    {
        if (!CanUseInputs) return;
        Events.Scurrying?.Invoke(ctx.ReadValueAsButton());
    }

    private void OnEnable()
    {
        Inputs.Player.Move.performed += PlayerMoved;
        Inputs.Player.Move.started += PlayerMoved;
        Inputs.Player.Move.canceled += PlayerMoved;

        Inputs.Player.Jump.performed += PlayerJumping;
        Inputs.Player.Jump.canceled += PlayerJumping;

        Inputs.Player.Climb.performed += PlayerClimbing;
        Inputs.Player.Climb.canceled += PlayerClimbing;

        Inputs.Player.Scurry.performed += PlayerScurrying;
        Inputs.Player.Scurry.canceled += PlayerScurrying;

        Inputs.Player.Enable();
    }

    private void OnDisable()
    {
        Inputs.Player.Move.performed -= PlayerMoved;
        Inputs.Player.Move.started -= PlayerMoved;
        Inputs.Player.Move.canceled -= PlayerMoved;

        Inputs.Player.Jump.performed -= PlayerJumping;
        Inputs.Player.Jump.canceled -= PlayerJumping;

        Inputs.Player.Climb.performed -= PlayerClimbing;
        Inputs.Player.Climb.canceled-= PlayerClimbing;

        Inputs.Player.Scurry.performed -= PlayerScurrying;
        Inputs.Player.Scurry.canceled-= PlayerScurrying;

        Inputs.Player.Disable();
    }

    private void Awake() => Inputs = new();
}
