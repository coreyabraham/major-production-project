using UnityEngine.Audio;

/// <summary>
/// A Class for referencing a target `Audio Mixer Group` <br/>
/// This Class gets the exposed "Volume" argument and dictates what type the Audio will run under during gameplay via the `SettingsUI.cs` script.
/// </summary>
[System.Serializable]
public class MixerReference
{
    public string ExposedName;
    public AudioType AudioType;
    public AudioMixerGroup AudioMixerGroup;
}