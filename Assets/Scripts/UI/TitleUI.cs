using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

public class TitleUI : MonoBehaviour
{
    [System.Serializable]
    public struct FrameGroup
    {
        public string Name;
        public GameObject Frame;
    }

    [field: Header("Values")]
    [field: SerializeField] private int StartingGroupIndex;

    [field: Header("Miscellaneous")]
    public string ButtonTag = "UI_BUTTON";

    [field: Header("External References")]
    public EventSystem EventSystem;
    public SettingsUI SettingsMenu;
    public ExitUI ExitMenu;

    [field: Header("Lists and Arrays")]
    [field: SerializeField] private FrameGroup[] Groups;
    
    [field: SerializeField] private List<NavigatorButton> CachedButons;

    private bool GameStarting = false;

    public void StartGame()
    {
        if (GameStarting) return;
        GameStarting = true;

        int targetIndex = GameSystem.Instance.GetCurrentSceneBuildIndex() + 1;
        string targetName = GameSystem.Instance.GetLevelNameWithIndex(targetIndex);

        GameSystem.Instance.RequestLoadScene(targetName);
    }

    private void GetButtons()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(ButtonTag);

        foreach (GameObject obj in objs)
        {
            NavigatorButton navigator = obj.GetComponent<NavigatorButton>();

            if (!navigator) continue;
            if (CachedButons.Contains(navigator)) continue;

            navigator.ClickedEvent.AddListener(() => ToggleFrames(navigator.TargetFrame));
            CachedButons.Add(navigator);
        }
    }

    private void ToggleFrames(GameObject Frame = null)
    {
        if (Frame == null) return;

        for (int i = 0; i < Groups.Length; i++)
        {
            if (Groups[i].Frame == null) continue;

            if (Groups[i].Frame == Frame)
            {
                Groups[i].Frame.SetActive(true);
                continue;
            }

            Groups[i].Frame.SetActive(false);
        }

        GetButtons();
    }

    private void Start() => ToggleFrames(Groups[StartingGroupIndex].Frame);
}
