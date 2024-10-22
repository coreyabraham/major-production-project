using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.Events;

public class DropdownUI : MonoBehaviour
{
    public TMP_Dropdown Dropdown;
    public TMP_Text TextLabel;

    [field: SerializeField] private int DropdownStartIndex = -1;
    [field: SerializeField] private DropdownTypeUI DropdownFillType;

    [field: Space(10.0f)]

    public UnityEvent<int> ValueChanged;
    public UnityEvent DropdownInitalized;

    [field: Space(10.0f)]

    [field: SerializeField] private List<TMP_Dropdown.OptionData> CustomListings;

    private void ValueModified(int value) => ValueChanged?.Invoke(value);

    private void Awake()
    {
        if (DropdownFillType != DropdownTypeUI.None) Dropdown.ClearOptions();

        // TODO: PLEASE REPLACE THIS IN THE FUTURE!
        static bool FindStringInList(string target, List<string> list)
        {
            bool found = false;

            foreach (string entry in list)
            {
                if (target != entry) continue;

                found = true;
                break;
            }

            return found;
        }

        switch (DropdownFillType)
        {
            case DropdownTypeUI.Custom:
                {
                    Dropdown.AddOptions(CustomListings);
                }
                break;

            case DropdownTypeUI.Resolution:
                {
                    Resolution[] resolutions = Screen.resolutions;
                    List<string> entries = new();

                    foreach (Resolution resolution in resolutions)
                    {
                        string str = resolution.width + "x" + resolution.height;

                        if (FindStringInList(str, entries)) continue;

                        entries.Add(str);
                    }

                    Dropdown.AddOptions(entries);
                }
                break;

            case DropdownTypeUI.WindowModes:
                {
                    string[] enums = System.Enum.GetNames(typeof(FullScreenMode));
                    List<string> options = new();

                    foreach (string str in enums)
                    {
                        options.Add(str);
                    }

                    Dropdown.AddOptions(options);
                }
                break;

            case DropdownTypeUI.HudScale:
                {
                    string[] enums = System.Enum.GetNames(typeof(HudScaleUI));
                    List<string> options = new();

                    foreach (string str in enums)
                    {
                        options.Add(str);
                    }

                    Dropdown.AddOptions(options);
                }
                break;
        }

        if (DropdownStartIndex > -1 || DropdownStartIndex < Dropdown.options.Count)
        {
            Dropdown.value = DropdownStartIndex;
            Dropdown.RefreshShownValue();
        }

        Dropdown.onValueChanged.AddListener(ValueModified);
        DropdownInitalized?.Invoke();
    }
}
