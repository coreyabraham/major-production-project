using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [field: SerializeField] private string TitleScreenScene = "Title Screen";
    [field: SerializeField] private string TitleUIObject = "TitleUI";
    [field: SerializeField] private string PlayUIObject = "PlayUI";

    [field: Space(2.5f)]

    [field: SerializeField] private GameObject Frame;
    [field: SerializeField] private SettingsUI Settings;
    [field: SerializeField] private NavigatorButton SettingsBackBtn;

    [field: Space(2.5f)]

    [field: SerializeField] private NavigatorButton ResumeBtn;
    [field: SerializeField] private NavigatorButton SettingsBtn;
    [field: SerializeField] private NavigatorButton ExitBtn;

    [field: Space(2.5f)]

    [field: SerializeField] private PromptDataUI ToMainMenuData;

    private bool PausingPermitted;

    public void NewSceneLoaded(Scene Scene, LoadSceneMode Mode)
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

        if (!TitleUI) return;
        if (!PlayUI) return;

        TitleUI.ExitMenu.PromptUI = Settings.PromptHandler;

        TitleUI.SettingsMenu.JsonHandler = Settings.JsonHandler;
        TitleUI.SettingsMenu.PromptHandler = Settings.PromptHandler;

        PlayUI.PromptSystem = Settings.PromptHandler;
    }

    public void InputCalled(InputAction.CallbackContext ctx)
    {
        if (!PausingPermitted) return;
        if (ctx.phase != InputActionPhase.Performed) return;
        ToggleUI();
    }

    private void ToggleUI()
    {
        Frame.SetActive(!Frame.activeSelf);

        if (Settings.gameObject.activeSelf) Settings.gameObject.SetActive(false);
        if (Settings.PromptHandler.PromptActive()) Settings.PromptHandler.ForceEnd();

        Time.timeScale = (Frame.activeSelf) ? 0.0f : 1.0f;
    }

    private void PromptFinalized(bool result)
    {
        if (!result) return;
        SceneManager.LoadScene(TitleScreenScene);
        ToggleUI();
    }

    private void SettingsBackBtnClicked()
    {
        if (!PausingPermitted) return;
        Settings.gameObject.SetActive(false);
        Frame.SetActive(true);
    }

    private void ResumeClicked() => ToggleUI();

    private void SettingsClicked()
    {
        Settings.gameObject.SetActive(true);
        Frame.SetActive(false);
    }

    private void ExitClicked()
    {
        ToMainMenuData.PromptFinalized = PromptFinalized;
        Settings.PromptHandler.Begin(ToMainMenuData);
    }

    private void Awake()
    {
        Frame.SetActive(false);

        ResumeBtn.ClickedEvent.AddListener(ResumeClicked);
        SettingsBtn.ClickedEvent.AddListener(SettingsClicked);
        ExitBtn.ClickedEvent.AddListener(ExitClicked);

        SettingsBackBtn.ClickedEvent.AddListener(SettingsBackBtnClicked);
    }
}
