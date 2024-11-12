using UnityEngine;
using TMPro;

public class TitleUI : MonoBehaviour
{
    [System.Serializable]
    public struct FrameGroup
    {
        public string Name;
        public GameObject Frame;
    }

    public TMP_Text SubTitle;
    public string SubTitleText = "TEAM NAME";
    public string ButtonTag = "UI_BUTTON";

    [field: Space(5.0f)]

    public SettingsUI SettingsMenu;
    public ExitUI ExitMenu;

    [field: Space(5.0f)]

    [field: SerializeField] private FrameGroup[] Groups;

    private void GetButtons()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(ButtonTag);

        foreach (GameObject obj in objs)
        {
            NavigatorButton navigator = obj.GetComponent<NavigatorButton>();

            if (!navigator) continue;

            navigator.ClickedEvent.AddListener(() => ToggleFrames(navigator.TargetFrame));
        }
    }

    private void ToggleFrames(GameObject Frame = null)
    {
        if (Frame == null) return;

        FrameGroup target = new();
        bool foundTarget = false;

        foreach (FrameGroup group in Groups)
        {
            if (group.Frame == Frame)
            {
                target = group;
                foundTarget = true;
                Frame.SetActive(true);

                continue;
            }

            group.Frame?.SetActive(false);
        }

        string str = (!foundTarget) ? Frame.name : target.Name;

        string combo = SubTitleText + " | ";
        if (string.IsNullOrWhiteSpace(SubTitleText)) combo = string.Empty;

        SubTitle.text = combo + str;

        GetButtons();
    }
    
    private void Start() => ToggleFrames(Groups[0].Frame);
}
