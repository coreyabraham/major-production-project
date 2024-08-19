using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [field: SerializeField] private GameObject Frame;

    [field: Space(2.5f)]

    [field: SerializeField] private NavigatorButton ResumeBtn;
    [field: SerializeField] private NavigatorButton SettingsBtn;
    [field: SerializeField] private NavigatorButton ExitBtn;

    public void InputCalled(InputAction.CallbackContext ctx) => ToggleUI();

    private void ToggleUI()
    {
        Frame.SetActive(!Frame.activeSelf);
        Time.timeScale = (Frame.activeSelf) ? 0.0f : 1.0f;
    }

    private void ResumeClicked() => ToggleUI();

    private void SettingsClicked()
    {
        print("Settings Clicked!");
    }

    private void ExitClicked()
    {
        // Insert Prompt Here, if Yes, Activate the following code:
        SceneManager.LoadScene("Title Screen");
        ToggleUI();
    }

    private void Awake()
    {
        Frame.SetActive(false);

        ResumeBtn.Button.onClick.AddListener(ResumeClicked);
        SettingsBtn.Button.onClick.AddListener(SettingsClicked);
        ExitBtn.Button.onClick.AddListener(ExitClicked);
    }
}
