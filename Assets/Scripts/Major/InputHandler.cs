using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : Singleton<InputHandler>
{
    [field: Header("External References")]
    [field: SerializeField] private InputActionAsset Actions;
    [field: SerializeField] private GameSystem GameSystem;

    [field: Header("Settings")]
    [field: SerializeField] private string ActionMap = "Player";
    public bool InputUsability = true;

    private InputActionMap InputActionMap;

    public bool CanToggleControls()
    {
        bool actionMap = InputActionMap != null;
        string current = GameSystem.GetCurrentLevelName();
        bool isBlacklistedScene = GameSystem.BlacklistedPauseScenes.Contains(current);

        return actionMap && isBlacklistedScene;
    }

    public void EnableControls()
    {
        if (!CanToggleControls()) return;
        InputActionMap.Enable();
    }

    public void DisableControls()
    {
        if (!CanToggleControls()) return;
        InputActionMap.Disable();
    }

    private void OnEnable() => EnableControls();

    private void OnDisable() => DisableControls();

    override protected void Initialize() => InputActionMap = Actions.FindActionMap(ActionMap, true);
}
 