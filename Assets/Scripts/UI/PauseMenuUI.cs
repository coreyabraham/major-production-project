using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [field: Header("Settings")]
    [field: SerializeField] private bool ToggleCursor = true;

    [field: Header("Scenes and Tags")]
    [field: SerializeField] private string TitleScreenScene = "Title Screen";
    [field: SerializeField] private string TitleUIObject = "TitleUI";

    [field: Header("Generics")]
    [field: SerializeField] private GameObject Frame;
    [field: SerializeField] private SettingsUI Settings;

    [field: Header("Lists and Arrays")]
    [field: SerializeField] private PromptDataUI ToMainMenuData;

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

        foreach (GameObject obj in Scene.GetRootGameObjects())
        {
            if (obj.name != TitleUIObject) continue;

            TitleUI title = obj.GetComponent<TitleUI>();
            if (!title) continue;

            TitleUI = title;
            break;
        }

        if (!TitleUI) return;

        TitleUI.ExitMenu.PromptUI = Settings.PromptHandler;

        TitleUI.SettingsMenu.JsonHandler = Settings.JsonHandler;
        TitleUI.SettingsMenu.PromptHandler = Settings.PromptHandler;
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

        bool levelCheck = GameSystem.Instance.BlacklistedPauseScenes.Contains(GameSystem.Instance.GetCurrentLevelName());
        if (!ToggleCursor || levelCheck) return;

        Cursor.visible = Frame.activeSelf;
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

    private void Start()
    {
        Frame.SetActive(false);

        bool levelCheck = GameSystem.Instance.BlacklistedPauseScenes.Contains(GameSystem.Instance.GetCurrentLevelName());
        if (!ToggleCursor || levelCheck) return;

        Cursor.visible = false;
    }
}
