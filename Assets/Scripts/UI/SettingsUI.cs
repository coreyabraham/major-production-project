using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [System.Serializable]
    public class SettingsGroup
    {
        public GameObject Frame;
        public NavigatorButton Navigator;
    }

    [field: SerializeField] private TMP_Text TitleLabel;

    [field: Space(5.0f)]

    [field: SerializeField] private string TitleText = "Player Settings";
    [field: SerializeField] private string DividerText = " // ";

    [field: Space(5.0f)]

    [field: SerializeField] private int VisibleIndexOnStartup;
    [field: SerializeField] private SettingsGroup[] Groups;

    #region "SliderUI References"
    public void MasterVolumeChanged(float value)
    {

    }

    public void MusicVolumeChanged(float value)
    {

    }

    public void SoundVolumeChanged(float value)
    {

    }
    #endregion

    #region Private Methods
    private void ChangeSubFrame(GameObject Frame)
    {
        foreach (SettingsGroup group in Groups)
        {
            group.Frame.SetActive(group.Frame == Frame);
        }

        TitleLabel.text = TitleText + DividerText + Frame.name;
    }

    private void Awake()
    {
        foreach (SettingsGroup group in Groups)
        {
            group.Navigator.Button.onClick.AddListener(() => ChangeSubFrame(group.Frame));
        }

        if (VisibleIndexOnStartup > Groups.Length || VisibleIndexOnStartup < 0) return;

        ChangeSubFrame(Groups[VisibleIndexOnStartup].Frame);
    }
    #endregion
}
