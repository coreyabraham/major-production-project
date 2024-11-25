using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class CreditEntryUI : MonoBehaviour
{
    [HideInInspector] public string MemberName;
    [HideInInspector] public string MemberRoles;
    [HideInInspector] public string MemberLink;
    [HideInInspector] public Sprite MemberIcon;

    [field: SerializeField] private TMP_Text NameLabel;
    [field: SerializeField] private TMP_Text RolesLabel;
    [field: SerializeField] private Image IconImage;

    private Button button;

    private void ButtonClicked() => Application.OpenURL(MemberLink);

    private void Awake()
    {
        NameLabel.text = (!string.IsNullOrWhiteSpace(MemberName)) ? MemberName : "N / A";
        RolesLabel.text = (!string.IsNullOrWhiteSpace(MemberRoles)) ? MemberRoles : "N / A";

        if (MemberIcon) IconImage.sprite = MemberIcon;

        bool result = TryGetComponent(out Button button);

        if (!result)
        {
            Debug.LogWarning(name + " | Could not find Button Component within Credits Entry UI: " + MemberName + "!", this);
            return;
        }

        button.onClick.AddListener(ButtonClicked);
    }
}
 