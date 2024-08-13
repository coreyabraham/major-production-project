using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputHandler : Singleton<InputHandler>
{
    [System.Serializable]
    public class InputMethod
    {
        public string Name;
        public bool IgnoreInputUsability;

        [field: Space(5.0f)]

        public InputValueType ValueType;
        public UnityEvent<object> Event;
    }

    [field: Header("Settings")]
    [field: SerializeField] private InputActionAsset Actions;
    [field: SerializeField] private string ActionMap = "Player";

    [field: Header("Generic")]
    public bool InputUsability = true;

    [field: Space(5.0f)]

    [field: SerializeField] private InputMethod[] Inputs;
    public Dictionary<string, InputMethod> InputMethods = new();

    private InputActionMap InputActionMap;

    public void EnableControls() => InputActionMap.Enable();
    public void DisableControls() => InputActionMap.Disable();

    public InputMethod GetInputMethod(string Name)
    {
        InputMethods.TryGetValue(Name, out InputMethod value);
        return value;
    }

    private void OnEnable()
    {
        EnableControls();

        foreach (InputAction action in InputActionMap.actions)
        {
            InputMethod Method = GetInputMethod(action.name);

            switch (action.type)
            {
                case InputActionType.Button:
                    {
                        action.performed += (InputAction.CallbackContext ctx) => InputCallback(ctx, Method);
                        action.canceled += (InputAction.CallbackContext ctx) => InputCallback(ctx, Method);
                    }
                    break;

                case InputActionType.PassThrough:
                    {
                        action.started += (InputAction.CallbackContext ctx) => InputCallback(ctx, Method);
                        action.canceled += (InputAction.CallbackContext ctx) => InputCallback(ctx, Method);
                    }
                    break;

                case InputActionType.Value:
                    {
                        action.performed += (InputAction.CallbackContext ctx) => InputCallback(ctx, Method);
                        action.started += (InputAction.CallbackContext ctx) => InputCallback(ctx, Method);
                        action.canceled += (InputAction.CallbackContext ctx) => InputCallback(ctx, Method);
                    }
                    break;
            }
        }
    }

    private void OnDisable()
    {
        DisableControls();

        foreach (InputAction action in InputActionMap.actions)
        {
            InputMethod Method = GetInputMethod(action.name);
            if (Method == null) continue;

            switch (action.type)
            {
                case InputActionType.Button:
                    {
                        action.performed -= (InputAction.CallbackContext ctx) => InputCallback(ctx, Method);
                        action.canceled -= (InputAction.CallbackContext ctx) => InputCallback(ctx, Method);
                    }
                    break;

                case InputActionType.PassThrough:
                    {
                        action.started -= (InputAction.CallbackContext ctx) => InputCallback(ctx, Method);
                        action.canceled -= (InputAction.CallbackContext ctx) => InputCallback(ctx, Method);
                    }
                    break;

                case InputActionType.Value:
                    {
                        action.performed -= (InputAction.CallbackContext ctx) => InputCallback(ctx, Method);
                        action.started -= (InputAction.CallbackContext ctx) => InputCallback(ctx, Method);
                        action.canceled -= (InputAction.CallbackContext ctx) => InputCallback(ctx, Method);
                    }
                    break;
            }
        }
    }

    private void InputCallback(InputAction.CallbackContext ctx, InputMethod method)
    {
        if (!InputUsability) return;

        switch (ctx.action.type)
        {
            case InputActionType.Button: method?.Event?.Invoke(ctx.ReadValueAsButton()); return;
            
            case InputActionType.PassThrough: method?.Event?.Invoke(ctx.ReadValueAsObject()); return;
            
            case InputActionType.Value:
                {
                    switch (method.ValueType)
                    {
                        case InputValueType.Int: method?.Event?.Invoke(ctx.ReadValue<int>()); break;
                        case InputValueType.Float: method?.Event?.Invoke(ctx.ReadValue<float>()); break;
                        case InputValueType.Double: method?.Event?.Invoke(ctx.ReadValue<double>()); break;
                        case InputValueType.Char: method?.Event?.Invoke(ctx.ReadValue<char>()); break;
                        case InputValueType.Byte: method?.Event?.Invoke(ctx.ReadValue<byte>()); break;
                        case InputValueType.Vector2: method?.Event?.Invoke(ctx.ReadValue<Vector2>()); break;
                        case InputValueType.Vector3: method?.Event?.Invoke(ctx.ReadValue<Vector3>()); break;
                        case InputValueType.Bool: method?.Event?.Invoke(ctx.ReadValue<bool>()); break;
                    }
                }
                return;
        }
    }

    override protected void Initialize()
    {
        foreach (InputMethod input in Inputs) InputMethods.Add(input.Name, input);
        InputActionMap = Actions.FindActionMap("Player", true);
    }
}
