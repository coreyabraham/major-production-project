using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class CreditEntryUI : MonoBehaviour
{
    public string MemberName;
    public string MemberRoles;
    public Sprite MemberIcon;

    [field: Space(5.0f)]

    [field: SerializeField] private TMP_Text NameLabel;
    [field: SerializeField] private TMP_Text RolesLabel;
    [field: SerializeField] private Image IconImage;

    private void Awake()
    {
        NameLabel.text = (!string.IsNullOrWhiteSpace(MemberName)) ? MemberName : "N / A";
        RolesLabel.text = (!string.IsNullOrWhiteSpace(MemberRoles)) ? MemberRoles : "N / A";

        if (!MemberIcon) return;
        IconImage.sprite = MemberIcon;
    }
}
 