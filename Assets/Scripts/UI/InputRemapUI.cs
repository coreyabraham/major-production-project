using UnityEngine;
using UnityEngine.InputSystem;

public class InputRemapUI : MonoBehaviour
{
    [field: Header("Settings")]
    [field: SerializeField] private float TimeoutInterval;

    [field: Header("References")]
    [field: SerializeField] private NavigatorButton[] RebinderButtons;

    private RebindData CachedData;
    private NavigatorButton CachedButton;

    private float CurrentTimeout;

    private InputActionRebindingExtensions.RebindingOperation RebindingOperation;

    private void AssignText(NavigatorButton NavigatorButton = null, RebindData RebindData = null)
    {
        if (!NavigatorButton) NavigatorButton = CachedButton;
        if (!RebindData) RebindData = CachedData;

        int binding = RebindData.ActionReference.action.GetBindingIndexForControl(RebindData.ActionReference.action.controls[0]);

        string humanReadableString = InputControlPath.ToHumanReadableString(
            RebindData.ActionReference.action.bindings[binding].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        string formattedText = NavigatorButton.DefaultText + " : " + humanReadableString;
        NavigatorButton.Label.text = formattedText;
    }

    private void RebindComplete()
    {
        AssignText();

        RebindingOperation.Dispose();
        InputHandler.Instance.EnableControls();

        CachedData.AwaitingInput = false;
        
        CachedData = null;
        CachedButton = null;
    }

    private void RebindButtonClicked(NavigatorButton Button)
    {
        RebindData Data = Button.gameObject.GetComponent<RebindData>();

        if (Data.AwaitingInput)
        {
            Debug.LogWarning(name + " | Button: " + Button.name + " is already awaiting an input!!!");
            return;
        }

        CachedButton = Button;
        CachedData = Data;

        InputHandler.Instance.DisableControls();

        Data.AwaitingInput = true;
        Button.Label.text = "<< Awaiting Input... >>";

        RebindingOperation = Data.ActionReference.action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindComplete())
            .Start();

        print(Data.ActionReference);
    }

    private void Update()
    {
        if (!CachedData || !CachedData.AwaitingInput) return;

        if (CurrentTimeout < TimeoutInterval)
        {
            CurrentTimeout += Time.deltaTime;
            return;
        }

        CurrentTimeout = 0.0f;
        RebindComplete();
    }

    private void Start()
    {
        //foreach (var test in InputHandler.Instance.InputActionMap.actions[0].bindings)
        //{
        //    print(test.name + " | " + test.path);
        //}

        // TODO: USE THIS TO APPLY BINDS ON STARTUP USING JSON ENCODING / DECODING IN `SettingsUI.cs`!!!
        InputHandler.Instance.InputActionMap.actions[1].ApplyBindingOverride("<Keyboard>/h");
    }

    private void Awake()
    {
        foreach (NavigatorButton button in RebinderButtons)
        {
            RebindData data = button.gameObject.GetComponent<RebindData>();

            if (!data)
            {
                Debug.LogWarning(name + " | Could not find `RebindData` in Button: " + button.name + "!");
                continue;
            }

            AssignText(button, data);
            button.ClickedEvent.AddListener(() => RebindButtonClicked(button));
        }
    }
}
