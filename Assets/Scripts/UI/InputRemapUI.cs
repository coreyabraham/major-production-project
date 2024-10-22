using System;
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

    private void ReassignValues(RebindData data)
    {
        print(data.ActionReference.action.bindings[0].path);

        if (data.CarriedBinds.Length != 0)
        {
            for (int i = 0; i < data.CarriedBinds.Length; i++)
            {
                data.CarriedBinds[i] = "";
            }
        }

        data.CarriedBinds = new string[data.ActionReference.action.bindings.Count];

        for (int i = 0; i < data.ActionReference.action.bindings.Count; i++)
        {
            data.CarriedBinds[i] = data.ActionReference.action.bindings[i].path;
        }
    }

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
        ReassignValues(CachedData);

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

            ReassignValues(data);
            AssignText(button, data);

            button.ClickedEvent.AddListener(() => RebindButtonClicked(button));
        }
    }
}
 