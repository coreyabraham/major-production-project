using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SliderUI : MonoBehaviour
{
    [field: SerializeField] private Slider Slider;
    [field: SerializeField] private TMP_Text Text;

    [field: Space(5.0f)]

    [field: SerializeField] private bool UpdateLabel;
    [field: SerializeField] private string BaseText = "{TEXT}";
    [field: SerializeField] private string TextExtension = "%";

    [field: Space(5.0f)]

    [field: SerializeField] private SliderTypeUI SliderDisplayType;

    [field: Space(5.0f)]

    [field: SerializeField] private UnityEvent<Single> ValueChanged;

    private void ValueModified(Single value, bool invokeEvent = false)
    {
        float displayValue = value;

        switch (SliderDisplayType)
        {
            case SliderTypeUI.Percentage:
                {
                    displayValue *= 100;
                }
                break;

            case SliderTypeUI.WholeNumber:
                {
                    value = Mathf.RoundToInt(value);
                    displayValue = value;
                }
                break;
        }

        if (UpdateLabel) Text.text = BaseText + ": " + displayValue.ToString() + TextExtension;

        ValueChanged?.Invoke(value);
    }

    private void Awake()
    {
        ValueModified(Slider.value);
        Slider.onValueChanged.AddListener((float value) => ValueModified(value, true));
    }
}
