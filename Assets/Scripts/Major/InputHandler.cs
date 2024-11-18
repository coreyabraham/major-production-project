using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : Singleton<InputHandler>
{
    [field: Header("External References")]
    [field: SerializeField] private InputActionAsset Actions;

    [field: Header("Settings")]
    [field: SerializeField] private string ActionMap = "Player";
    public bool InputUsability = true;

    private InputActionMap InputActionMap;

    public void EnableControls()
    {
        if (InputActionMap == null) return;
        InputActionMap.Enable();
    }

    public void DisableControls()
    {
        if (InputActionMap == null) return;
        InputActionMap.Disable();
    }

    private void OnEnable() => EnableControls();

    private void OnDisable() => DisableControls();

    override protected void Initialize() => InputActionMap = Actions.FindActionMap(ActionMap, true);
}
 