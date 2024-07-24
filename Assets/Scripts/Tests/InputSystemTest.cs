using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputSystemTest : MonoBehaviour
{
    [System.Serializable]
    public class InputEvents
    {
        public UnityEvent<Vector2> Moving;
    }

    [field: SerializeField] private InputEvents Events;
    private Actions Inputs;

    public void Callback(Vector2 value) => print(value.ToString());

    public void PlayerMoved(InputAction.CallbackContext ctx) => Events.Moving?.Invoke(ctx.ReadValue<Vector2>());

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
