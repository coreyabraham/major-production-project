using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;

public class AudioHandler : Singleton<AudioHandler>
{
    [field: Header("Mixing")]
    public AudioMixer AudioMixer;
    public MixerReference[] MixerReferences;

    [field: Header("Extenal References")]
    [field: SerializeField] private JSONData JsonHandler;

    [field: Header("Lists and Arrays")]
    [field: SerializeField] private List<AudioSource> Sources = new();

    public void AddSource(AudioSource Source) => Sources.Add(Source);
    public bool RemoveSource(AudioSource Source) => Sources.Remove(Source);

    public MixerReference GetMixerReference(AudioType TargetAudioType)
    {
        // TODO: Surely there's a better way to do this... right?
        foreach (MixerReference mixer in MixerReferences)
        {
            if (mixer.AudioType != TargetAudioType) continue;
            return mixer;
        }

        return null;
    }

    public AudioSource GetSource(string Name)
    {
        // TODO: Replace this algorithm with a much faster find Algorithm
        AudioSource found = null;

        foreach (AudioSource source in Sources)
        {
            if (source.name != Name) continue;
            found = source;
        }

        if (!found)
        {
            Debug.LogWarning(name + " | Could not find AudioSource with Name: " + Name + " in internal AudioSources list.");
            return null;
        }

        return found;
    }

    public AudioSource CreateGlobalSource(AudioClip Clip, AudioType AudioType)
    {
        GameObject container = new();
        container.name = Clip.name;
        container.transform.SetParent(gameObject.transform);

        AudioSource source = container.AddComponent<AudioSource>();

        source.name = Clip.name;
        source.clip = Clip;
        source.playOnAwake = false;

        if (AudioType != AudioType.None) source.outputAudioMixerGroup = GetMixerReference(AudioType).AudioMixerGroup;

        AddSource(source);
        return source;
    }

    public AudioSource CreateSourceOnObject(GameObject Container, AudioClip Clip, AudioType AudioType)
    {
        AudioSource source = Container.AddComponent<AudioSource>();

        source.name = Clip.name;
        source.clip = Clip;
        source.playOnAwake = false;

        if (AudioType != AudioType.None) source.outputAudioMixerGroup = GetMixerReference(AudioType).AudioMixerGroup;

        AddSource(source);
        return source;
    }

    public void PlayWithType(AudioSource Source, AudioPlaybackType PlaybackType, float Value)
    {
        switch (PlaybackType)
        {
            case AudioPlaybackType.Play: Source.Play(); break;
            case AudioPlaybackType.PlayDelayed: Source.PlayDelayed(Value); break;
            case AudioPlaybackType.PlayOneShot: Source.PlayOneShot(Source.clip, Value); break;
            case AudioPlaybackType.PlayScheduled: Source.PlayScheduled((double)Value); break;

            case AudioPlaybackType.Pause: Source.Pause(); break;
            case AudioPlaybackType.Stop: Source.Stop(); break;
        }
    }

    public void PlaySource(AudioSource Source, ulong delay = 0) => Source.Play(delay);

    public void PlaySource(string Name, ulong delay = 0)
    {
        AudioSource source = GetSource(Name);
        if (!source) return;
        PlaySource(source, delay);
    }

    public float AudioCalculations(float value) => Mathf.Log10(value) * 20.0f;

    private void Start()
    {
        if (JsonHandler == null) return;

        PlayerSettings currentData = JsonHandler.GetCurrentData();

        for (int i = 0; i < MixerReferences.Length; i++)
        {
            if (!AudioMixer.GetFloat(MixerReferences[i].ExposedName, out float _)) continue;
            float value = 0.0f;

            switch (MixerReferences[i].AudioType)
            {
                case AudioType.None: break;
                case AudioType.Master: value = currentData.MasterVolume; break;
                case AudioType.Music: value = currentData.MusicVolume; break;
                case AudioType.Sound: value = currentData.SoundVolume; break;
            }

            AudioMixer.SetFloat(MixerReferences[i].ExposedName, AudioCalculations(value));
        }
    }

    protected override void Initialize() 
    {
        // TODO: Figure out what needs to be done here in future revisions!
        //foreach (AudioSource source in Sources)
        //{

        //}
    }
}
