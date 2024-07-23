using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [System.Serializable]
    public class InputEvents
    {
        public UnityEvent<Vector2> Moving;
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

    private void OnEnable()
    {
        Inputs.Player.Move.performed += PlayerMoved;

        Inputs.Player.Enable();
    }

    private void OnDisable()
    {
        // Events Go Here!

        Inputs.Player.Disable();
    }

    private void Awake() => Inputs = new();
}
