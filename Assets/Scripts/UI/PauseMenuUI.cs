using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [field: Header("Settings")]
    [field: SerializeField] private bool ToggleCursor = false;
    [field: SerializeField] private bool AllowForDebug = true;
    [field: SerializeField] private bool DemoResetKey = false;

    [field: Header("Scenes and Tags")]
    [field: SerializeField] private string TitleScreenScene = "Title Screen";
    [field: SerializeField] private string TitleUIObject = "TitleUI";

    [field: Header("Generics")]
    [field: SerializeField] private GameObject Frame;
    [field: SerializeField] private SettingsUI Settings;
    [field: SerializeField] private GameObject SettingsContainer;
    [field: SerializeField] private TMP_Text FPSLabel;

    [field: Header("Lists and Arrays")]
    [field: SerializeField] private PromptDataUI ToMainMenuData;

    private bool PausingPermitted;
    private float timer, refresh, avgFramerate;

    public void NewSceneLoaded(Scene Scene, LoadSceneMode _)
    {
        PausingPermitted = !GameSystem.Instance.BlacklistedPauseScenes.Contains(Scene.name);
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

    public void PauseInputCalled(InputAction.CallbackContext ctx)
    {
        if (!PausingPermitted || ctx.phase != InputActionPhase.Performed) return;
        ToggleUI();
    }

    public void ResetInputCalled(InputAction.CallbackContext ctx)
    {
        bool levelCheck = GameSystem.Instance.BlacklistedPauseScenes.Contains(GameSystem.Instance.GetCurrentLevelName());
        if (!DemoResetKey || levelCheck || ctx.phase != InputActionPhase.Performed) return;
        GameSystem.Instance.RequestLoadScene(TitleScreenScene);
    }

    public void DebugInputCalled(InputAction.CallbackContext ctx)
    {
        if (!AllowForDebug || ctx.phase != InputActionPhase.Performed) return;
        FPSLabel.gameObject.SetActive(!FPSLabel.gameObject.activeSelf);
    }

    private void ToggleUI()
    {
        Frame.SetActive(!Frame.activeSelf);

        if (SettingsContainer.activeSelf) SettingsContainer.SetActive(false);
        if (Settings.PromptHandler.PromptActive()) Settings.PromptHandler.ForceEnd();

        GameSystem.Instance.SetPausedState(Frame.activeSelf);
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
        SettingsContainer.SetActive(false);
        Frame.SetActive(true);
    }

    public void ResumeClicked() => ToggleUI();

    public void SettingsClicked()
    {
        SettingsContainer.SetActive(true);
        Frame.SetActive(false);
    }

    public void ExitClicked()
    {
        ToMainMenuData.PromptFinalized = PromptFinalized;
        Settings.PromptHandler.Begin(ToMainMenuData);
    }

    private void Update()
    {
        if (!FPSLabel.gameObject.activeSelf) return;

        if (Time.timeScale <= 0.0f)
        {
            FPSLabel.text = "Game Paused.";
            return;
        }

        float timelapse = Time.smoothDeltaTime;
        timer = timer <= 0 ? refresh : timer -= timelapse;

        if (timer <= 0) avgFramerate = (int)(1f / timelapse);
        FPSLabel.text = string.Format("FPS: {0}", avgFramerate.ToString());
    }

    private void Start()
    {
        Frame.SetActive(false);

        bool levelCheck = GameSystem.Instance.BlacklistedPauseScenes.Contains(GameSystem.Instance.GetCurrentLevelName());
        if (!ToggleCursor || levelCheck) return;

        Cursor.visible = false;
    }
}
