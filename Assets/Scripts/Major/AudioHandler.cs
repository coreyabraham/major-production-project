using UnityEngine;
using UnityEngine.Audio;

public class AudioHandler : MonoBehaviour
{
    [field: SerializeField] private JSONData JsonHandler;
    
    public AudioMixerGroup AudioMixer;
    public MixerReference[] MixerReferences;

    public float AudioCalculations(float value) => Mathf.Log10(value) * 20.0f;

    private void Start()
    {
        PlayerSettings currentData = JsonHandler.GetCurrentData();

        for (int i = 0; i < MixerReferences.Length; i++)
        {
            if (!AudioMixer.audioMixer.GetFloat(MixerReferences[i].ExposedName, out float _)) continue;
            float value = 0.0f;

            switch (MixerReferences[i].AudioType)
            {
                case AudioType.None: break;
                case AudioType.Master: value = currentData.MasterVolume; break; 
                case AudioType.Music: value = currentData.MusicVolume; break; 
                case AudioType.Sound: value = currentData.SoundVolume; break; 
            }

            AudioMixer.audioMixer.SetFloat(MixerReferences[i].ExposedName, AudioCalculations(value));
        }
    }
}
