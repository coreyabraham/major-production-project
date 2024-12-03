using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class EndingUI : MonoBehaviour
{
    [field: Header("Strings")]
    [field: SerializeField] private string TitleSceneName = "Main Menu";
    [field: SerializeField] private string AboveZeroDeaths = "You killed {DEATHS} Rats during the game... yikes.";
    [field: SerializeField] private string ZeroDeaths = "You didn't kill a single Rat! Horray for you!";

    [field: Header("Public References")]
    public Image Background;

    [field: Header("Internal References")]
    [field: SerializeField] private GameObject Main;
    [field: SerializeField] private TMP_Text Text;
    [field: SerializeField] private NavigatorButton[] Buttons;

    private bool AlreadyClicked = false;
    private uint RatDeaths = 0;

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
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                Application.Quit();
                break;
        }
    }

    public void EnableUI()
    {
        RatDeaths = GameSystem.Instance.PlayerDeathCount;

        GameSystem.Instance.Player.PhysicsPaused = true;
        GameSystem.Instance.Player.SetMoveType(MoveType.None);

        string chosen = RatDeaths > 0 ? AboveZeroDeaths : ZeroDeaths;

        if (chosen == AboveZeroDeaths && RatDeaths == 1)
            chosen = chosen.Replace("Rats", "Rat");

        chosen = chosen.Replace("{DEATHS}", RatDeaths.ToString());

        Text.text = chosen;
        Main.SetActive(true);

        foreach (NavigatorButton button in Buttons)
        {
            button.ClickedEvent.AddListener(() => ButtonClicked(button));
            button.gameObject.SetActive(true);
        }
    }
}
