using UnityEngine;

using UnityEngine.EventSystems;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [field: Header("Scenes and Tags")]
    [field: SerializeField] private string TitleScreenScene = "Title Screen";
    [field: SerializeField] private string TitleUIObject = "TitleUI";
    [field: SerializeField] private string PlayUIObject = "PlayUI";

    [field: Header("Generics")]
    [field: SerializeField] private GameObject Frame;
    [field: SerializeField] private SettingsUI Settings;

    [field: Header("Navigation")]
    [field: SerializeField] private NavigatorButton ResumeBtn;

    [field: Header("Lists and Arrays")]
    [field: SerializeField] private PromptDataUI ToMainMenuData;

    [field: SerializeField] private string[] ActionNames = {
        "Submit",
        "Click"
    };

    [field: Header("Externals")]
    [field: SerializeField] private InputSystemUIInputModule InputModule;

    private bool PausingPermitted;

    // TODO: Improve ALL searching algorithms contained within this method!
    public void NewSceneLoaded(Scene Scene, LoadSceneMode _)
    {
        bool result = false;

        foreach (string SceneName in GameSystem.Instance.BlacklistedPauseScenes)
        {
            result = Scene.name != SceneName;
            if (result == true) break;
        }

        PausingPermitted = result;

        if (Scene.name != TitleScreenScene) return;

        TitleUI TitleUI = null;
        PlayUI PlayUI = null;

        foreach (GameObject obj in Scene.GetRootGameObjects())
        {
            if (obj.name != TitleUIObject) continue;

            TitleUI title = obj.GetComponent<TitleUI>();
            if (!title) continue;

            TitleUI = title;
            break;
        }

        foreach (GameObject obj in Scene.GetRootGameObjects())
        {
            if (obj.name != PlayUIObject) continue;

            PlayUI play = obj.GetComponent<PlayUI>();
            if (!play) continue;

            PlayUI = play;
            break;
        }

        if (!TitleUI || !PlayUI) return;

        TitleUI.ExitMenu.PromptUI = Settings.PromptHandler;

        TitleUI.SettingsMenu.JsonHandler = Settings.JsonHandler;
        TitleUI.SettingsMenu.PromptHandler = Settings.PromptHandler;

        PlayUI.PromptSystem = Settings.PromptHandler;
    }

    public void InputCalled(InputAction.CallbackContext ctx)
    {
        if (!PausingPermitted || ctx.phase != InputActionPhase.Performed) return;
        ToggleUI();
    }

    private void ToggleUI()
    {
        Frame.SetActive(!Frame.activeSelf);

        if (Settings.gameObject.activeSelf) Settings.gameObject.SetActive(false);
        if (Settings.PromptHandler.PromptActive()) Settings.PromptHandler.ForceEnd();

        Time.timeScale = Frame.activeSelf ? 0.0f : 1.0f;
        EventSystem.current.firstSelectedGameObject = Frame.activeSelf ? ResumeBtn.Button.gameObject : null;
    }

    private void PromptFinalized(bool result)
    {
        if (!result) return;
        GameSystem.Instance.RequestLoadScene(TitleScreenScene);
        ToggleUI();
    }

    public void SettingsBackBtnClicked()
    {
        if (!PausingPermitted) return;
        Settings.gameObject.SetActive(false);
        Frame.SetActive(true);
    }

    public void ResumeClicked() => ToggleUI();

    public void SettingsClicked()
    {
        Settings.gameObject.SetActive(true);
        Frame.SetActive(false);
    }

    public void ExitClicked()
    {
        ToMainMenuData.PromptFinalized = PromptFinalized;
        Settings.PromptHandler.Begin(ToMainMenuData);
    }

    private void InputReceived(InputAction.CallbackContext ctx)
    {
        GameObject currentSelection = EventSystem.current.currentSelectedGameObject;
        if (!currentSelection) return;

        bool result = currentSelection.transform.parent.gameObject.TryGetComponent(out NavigatorButton navigatorButton);
        if (!result) return;

        navigatorButton.ClickedEvent?.Invoke();
    }

    private void Awake() => Frame.SetActive(false);

    private void OnEnable()
    {
        var map = InputModule.actionsAsset.FindActionMap("UI");

        foreach (string str in ActionNames)
        {
            var action = map.FindAction(str);
            if (action == null) continue;

            action.performed += InputReceived;
        }
    }
}
