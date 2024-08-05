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

    [field: SerializeField] private JSONData JsonHandler;
    [field: SerializeField] private PromptUI PromptHandler;

    [field: Space(5.0f)]

    [field: SerializeField] PromptDataUI SavePromptData;
    [field: SerializeField] PromptDataUI ResetPromptData;

    [field: Space(5.0f)]

    [field: SerializeField] private TMP_Text TitleLabel;

    [field: Space(5.0f)]

    [field: SerializeField] private string TitleText = "Player Settings";
    [field: SerializeField] private string DividerText = " // ";

    [field: Space(5.0f)]

    [field: SerializeField] private int VisibleIndexOnStartup;
    [field: SerializeField] private SettingsGroup[] Groups;

    [field: Space(5.0f)]

    [field: SerializeField] private SliderUI MasterSlider;
    [field: SerializeField] private SliderUI MusicSlider;
    [field: SerializeField] private SliderUI SoundSlider;

    [field: Space(2.5f)]

    [field: SerializeField] private DropdownUI ResolutionDD;
    [field: SerializeField] private DropdownUI QualityDD;
    [field: SerializeField] private DropdownUI WindowDD;
    [field: SerializeField] private DropdownUI HudScaleDD;

    [field: Space(2.5f)]

    [field: SerializeField] private SliderUI FPSSlider;
    [field: SerializeField] private CheckboxUI VsyncCB;

    [field: Space(5.0f)]

    [field: SerializeField] private NavigatorButton SaveBtn;
    [field: SerializeField] private NavigatorButton ResetBtn;

    #region Button References
    public void SaveButtonClicked() => PromptHandler.Begin(SavePromptData);
    public void ResetButtonClicked() => PromptHandler.Begin(ResetPromptData);
    #endregion

    #region SliderUI References
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

    #region PromptMethods
    private void SavePromptFinished(bool Result)
    {
        if (!Result) return;

        PlayerSettings newSettings = new()
        {
            MasterVolume = MasterSlider.Slider.value,
            MusicVolume = MusicSlider.Slider.value,
            SoundVolume = SoundSlider.Slider.value,

            Resolution = ResolutionDD.Dropdown.value,
            Quality = QualityDD.Dropdown.value,
            Fullscreen = WindowDD.Dropdown.value,
            HudScale = HudScaleDD.Dropdown.value,

            FramesPerSecond = (int)FPSSlider.Slider.value,
            UseVsync = VsyncCB.Checkbox.isOn
        };

        JsonHandler.ApplyData(newSettings);
    }

    private void ResetPromptFinished(bool Result)
    {
        print(Result);
    }
    #endregion

    #region Private Methods
    private void UpdateUI(PlayerSettings Settings)
    {
        MasterSlider.Slider.value = Settings.MasterVolume;
        MusicSlider.Slider.value = Settings.MusicVolume;
        SoundSlider.Slider.value = Settings.SoundVolume;

        ResolutionDD.Dropdown.value = Settings.Resolution;
        QualityDD.Dropdown.value = Settings.Quality;
        WindowDD.Dropdown.value = Settings.Fullscreen;
        HudScaleDD.Dropdown.value = Settings.HudScale;

        FPSSlider.Slider.value = Settings.FramesPerSecond;
        VsyncCB.Checkbox.isOn = Settings.UseVsync;
    }

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
        SavePromptData.PromptFinalized = SavePromptFinished;
        ResetPromptData.PromptFinalized = ResetPromptFinished;

        UpdateUI(JsonHandler.GetCurrentData());

        foreach (SettingsGroup group in Groups)
        {
            group.Navigator.Button.onClick.AddListener(() => ChangeSubFrame(group.Frame));
        }

        if (VisibleIndexOnStartup > Groups.Length || VisibleIndexOnStartup < 0) return;

        ChangeSubFrame(Groups[VisibleIndexOnStartup].Frame);
    }
    #endregion
}
