using System.Collections.Generic;

using UnityEngine;

using UnityEngine.EventSystems;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class TitleUI : MonoBehaviour
{
    [System.Serializable]
    public struct FrameGroup
    {
        public string Name;
        public GameObject Frame;
        public GameObject FirstSelection;
    }

    [field: Header("Values")]
    [field: SerializeField] private int StartingGroupIndex;

    [field: Header("Miscellaneous")]
    public string ButtonTag = "UI_BUTTON";

    [field: Header("External References")]
    public EventSystem EventSystem;
    public SettingsUI SettingsMenu;
    public ExitUI ExitMenu;

    [field: SerializeField] private InputSystemUIInputModule InputModule;

    [field: Header("Lists and Arrays")]
    [field: SerializeField] private FrameGroup[] Groups;
    
    [field: SerializeField] private string[] ActionNames = {
        "Submit",
        "Click"
    };

    [field: SerializeField] private List<NavigatorButton> CachedButons;

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

                GameObject selection = Groups[i].FirstSelection;
                
                EventSystem.current.firstSelectedGameObject = selection;
                //EventSystem.current.SetSelectedGameObject(selection);
                
                continue;
            }

            Groups[i].Frame.SetActive(false);
        }

        GetButtons();
    }

    private void InputReceived(InputAction.CallbackContext ctx)
    {
        if (ctx.phase != InputActionPhase.Performed) return;

        GameObject currentSelection = EventSystem.current.currentSelectedGameObject;
        if (!currentSelection) return;

        bool result = currentSelection.transform.parent.gameObject.TryGetComponent(out NavigatorButton navigatorButton);
        if (!result) return;

        navigatorButton.ClickedEvent?.Invoke();
    }

    private void Start() => ToggleFrames(Groups[StartingGroupIndex].Frame);

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
