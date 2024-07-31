using TMPro;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CheckboxUI : MonoBehaviour
{
    [field: SerializeField] private Toggle Checkbox;
    [field: SerializeField] private TMP_Text TextLabel;

    [field: SerializeField] private UnityEvent<bool> CheckboxToggled;

    private void Toggled(bool value) => CheckboxToggled?.Invoke(value);

    private void Awake() => Checkbox.onValueChanged.AddListener(Toggled);
}
