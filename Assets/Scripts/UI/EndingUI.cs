using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EndingUI : MonoBehaviour
{
    [field: Header("Miscellaneous")]
    [field: SerializeField] private string TitleSceneName = "Main Menu";
    
    [field: Header("Public References")]
    public Image Background;

    [field: Header("Internal References")]
    [field: SerializeField] private GameObject Main;
    [field: SerializeField] private NavigatorButton[] Buttons;

    private bool AlreadyClicked = false;

    private void ButtonClicked(NavigatorButton Button)
    {
        switch (Button.name)
        {
            case "TitleBtn":
                if (AlreadyClicked) return;
                AlreadyClicked = true;

                GameSystem.Instance.RequestLoadScene(TitleSceneName);
                break;
            
            case "CloseBtn":
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#endif
                Application.Quit();
                break;
        }
    }

    public void ActivateUI()
    {
        foreach (NavigatorButton button in Buttons)
        {
            button.ClickedEvent.AddListener(() => ButtonClicked(button));
            button.gameObject.SetActive(true);
        }
    }

    private void OnEnable()
    {
        Main.SetActive(true);
    }
}
