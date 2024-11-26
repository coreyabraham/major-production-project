using System;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

public class SettingsUI : MonoBehaviour
{
    #region Classes
    [System.Serializable]
    public class SettingsGroup
    {
        public GameObject Frame;
        public NavigatorButton Navigator;
    }
    #endregion

    #region Variables
    [field: Header("External References")]
    public JSONData JsonHandler;
    public PromptUI PromptHandler;

    [field: Header("Sliders")]
    [field: SerializeField] private SliderUI MasterSlider;
    [field: SerializeField] private SliderUI MusicSlider;
    [field: SerializeField] private SliderUI SoundSlider;
    [field: SerializeField] private SliderUI FPSSlider;

    [field: Header("Dropdowns")]
    [field: SerializeField] private DropdownUI ResolutionDD;
    [field: SerializeField] private DropdownUI QualityDD;
    [field: SerializeField] private DropdownUI WindowDD;

    [field: Header("Checkboxes")]
    [field: SerializeField] private CheckboxUI VsyncCB;

    [field: Header("Miscellaneous")]
    [field: SerializeField] private TMP_Text TitleLabel;
    [field: SerializeField] private string TitleText = "Player Settings";
    [field: SerializeField] private string DividerText = " // ";
    [field: SerializeField] private GameObject Container;

    [field: Header("Prompt UI Data")]
    [field: SerializeField] PromptDataUI SavePromptData;
    [field: SerializeField] PromptDataUI ResetPromptData;

    [field: Header("Groups")]
    [field: SerializeField] private int VisibleIndexOnStartup;
    [field: SerializeField] private SettingsGroup[] Groups;

    [field: Header("Audio")]
    [field: SerializeField] private AudioClip SoundTestClip;
    private AudioSource SoundTest;
    #endregion

    #region Button References
    public void SaveButtonClicked()
    {
        Container.SetActive(false);
        PromptHandler.Begin(SavePromptData);
    }
    public void ResetButtonClicked()
    {
        Container.SetActive(false);
        PromptHandler.Begin(ResetPromptData);
    }
    #endregion

    #region Public References
    public void MasterVolumeChanged(float value)
    {
        MixerReference reference = AudioHandler.Instance.GetMixerReference(AudioType.Master);
        AudioHandler.Instance.AudioMixer.SetFloat(reference.ExposedName, AudioHandler.Instance.AudioDecibelCalculation(value));
    }

    public void MusicVolumeChanged(float value)
    {
        MixerReference reference = AudioHandler.Instance.GetMixerReference(AudioType.Music);
        AudioHandler.Instance.AudioMixer.SetFloat(reference.ExposedName, AudioHandler.Instance.AudioDecibelCalculation(value));
    }

    public void SoundVolumeChanged(float value)
    {
        MixerReference reference = AudioHandler.Instance.GetMixerReference(AudioType.Sound);
        AudioHandler.Instance.AudioMixer.SetFloat(reference.ExposedName, AudioHandler.Instance.AudioDecibelCalculation(value));

        if (SoundTest.isPlaying) SoundTest.Stop();
        SoundTest.Play();
    }

    public void VsyncToggled(bool IsOn) => FPSSlider.Slider.interactable = !IsOn;
    #endregion

    #region Prompt Methods
    private void SavePromptFinished(bool Result)
    {
        Container.SetActive(true);

        if (!Result) return;

        PlayerSettings newSettings = new()
        {
            MasterVolume = MasterSlider.Slider.value,
            MusicVolume = MusicSlider.Slider.value,
            SoundVolume = SoundSlider.Slider.value,

            Resolution = ResolutionDD.Dropdown.options[ResolutionDD.Dropdown.value].text,
            Quality = QualityDD.Dropdown.options[QualityDD.Dropdown.value].text,
            WindowMode = WindowDD.Dropdown.options[WindowDD.Dropdown.value].text,

            FramesPerSecond = (int)FPSSlider.Slider.value,
            UseVsync = VsyncCB.Checkbox.isOn,
        };

        JsonHandler.ApplyData(newSettings);
        UpdateGraphics();
    }

    private void ResetPromptFinished(bool Result)
    {
        Container.SetActive(true);

        if (!Result) return;

        PlayerSettings defaults = JsonHandler.GetDefaultData();
        JsonHandler.ApplyData(defaults);
        
        UpdateUI(defaults);
        UpdateGraphics();
    }
    #endregion

    #region Private Methods
    private int GetDropdownOptionIndex(List<TMP_Dropdown.OptionData> Options, string Text)
    {
        int index = -1;

        for (int i = 0; i < Options.Count; i++)
        {
            if (Options[i].text != Text) continue;
            
            index = i;
            break;
        }

        return index;
    }

    private void DropdownInitalized(TMP_Dropdown Dropdown, string Text)
    {
        Dropdown.value = GetDropdownOptionIndex(Dropdown.options, Text);
    }

    private void UpdateUI(PlayerSettings Settings)
    {
        MasterSlider.Slider.value = Settings.MasterVolume;
        MusicSlider.Slider.value = Settings.MusicVolume;
        SoundSlider.Slider.value = Settings.SoundVolume;

        ResolutionDD.Dropdown.value = GetDropdownOptionIndex(ResolutionDD.Dropdown.options, Settings.Resolution);
        QualityDD.Dropdown.value = GetDropdownOptionIndex(QualityDD.Dropdown.options, Settings.Quality);
        WindowDD.Dropdown.value = GetDropdownOptionIndex(WindowDD.Dropdown.options, Settings.WindowMode);

        FPSSlider.Slider.value = Settings.FramesPerSecond;
        VsyncCB.Checkbox.isOn = Settings.UseVsync;
    }

    private void UpdateGraphics()
    {
        PlayerSettings settings = JsonHandler.GetCurrentData();
        string[] strings = settings.Resolution.Split('x');

        if (strings.Length != 2)
        {
            string currentResolution = ResolutionDD.Dropdown.options[ResolutionDD.Dropdown.value].text;
            strings = currentResolution.Split('x');
        }

        int.TryParse(strings[0], out int width);
        int.TryParse(strings[1], out int height);

        Enum.TryParse<FullScreenMode>(settings.WindowMode, false, out FullScreenMode windowMode);

        string qualityStr = settings.Quality;
        qualityStr = qualityStr.Replace(" ", string.Empty);

        Enum.TryParse<VisualQuality>(qualityStr, false, out VisualQuality qualityLevel);

        Screen.SetResolution(width, height, windowMode);
        QualitySettings.SetQualityLevel((int)qualityLevel);

        if (settings.UseVsync)
        {
            QualitySettings.vSyncCount = 1;
            return;
        }

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = settings.FramesPerSecond;
    }

    private void ChangeSubFrame(GameObject Frame)
    {
        foreach (SettingsGroup group in Groups) group.Frame.SetActive(group.Frame == Frame);
        TitleLabel.text = TitleText + DividerText + Frame.name;
    }

    private void Start()
    {
        SavePromptData.PromptFinalized = SavePromptFinished;
        ResetPromptData.PromptFinalized = ResetPromptFinished;

        PlayerSettings currentData = JsonHandler.GetCurrentData();
        
        UpdateUI(currentData);
        UpdateGraphics();

        ResolutionDD.DropdownInitalized.AddListener(() => DropdownInitalized(ResolutionDD.Dropdown, currentData.Resolution));
        QualityDD.DropdownInitalized.AddListener(() => DropdownInitalized(QualityDD.Dropdown, currentData.Quality));
        WindowDD.DropdownInitalized.AddListener(() => DropdownInitalized(WindowDD.Dropdown, currentData.WindowMode));

        foreach (SettingsGroup group in Groups) group.Navigator.Button.onClick.AddListener(() => ChangeSubFrame(group.Frame));

        if (VisibleIndexOnStartup > Groups.Length || VisibleIndexOnStartup < 0) return;

        ChangeSubFrame(Groups[VisibleIndexOnStartup].Frame);

        SoundTest = AudioHandler.Instance.CreateGlobalSource(SoundTestClip, AudioType.Sound);
    }
    #endregion
}
