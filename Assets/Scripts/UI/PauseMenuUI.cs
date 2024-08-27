using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [field: SerializeField] private string TitleScreenScene = "Title Screen";
    [field: SerializeField] private bool ExcludePausingFromTS = true;

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
    
    public void InputCalled(InputAction.CallbackContext ctx)
    {
        // PLEASE UNHARD CODE THIS AND ALLOW ANY SCENE TO DISABLE PAUSING!
        if (ExcludePausingFromTS && SceneManager.GetActiveScene().name == TitleScreenScene) return;
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

        ResumeBtn.Button.onClick.AddListener(ResumeClicked);
        SettingsBtn.Button.onClick.AddListener(SettingsClicked);
        ExitBtn.Button.onClick.AddListener(ExitClicked);

        SettingsBackBtn.Button.onClick.AddListener(SettingsBackBtnClicked);
    }
}
