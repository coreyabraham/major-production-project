using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : Singleton<InputHandler>
{
    // Heavily reduced in size due to `PlayerInput` Instances being used instead,
    // this is used for global input settings now!

    [field: Header("Settings")]
    [field: SerializeField] private InputActionAsset Actions;
    [field: SerializeField] private string ActionMap = "Player";

    [field: Header("Generic")]
    public bool InputUsability = true;

    private InputActionMap InputActionMap;

    public void EnableControls() => InputActionMap.Enable(); // "Object reference not set to an instance of an object"
    public void DisableControls() => InputActionMap.Disable(); // "Object reference not set to an instance of an object"

    private void OnEnable() => EnableControls();

    private void OnDisable() => DisableControls();

    override protected void Initialize() => InputActionMap = Actions.FindActionMap(ActionMap, true);
}
