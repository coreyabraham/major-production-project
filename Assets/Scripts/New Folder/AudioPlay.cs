using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    [field: SerializeField] private AudioSource[] Sources;

    public void PlaySteps()
    {
        foreach (AudioSource source in Sources)
            source.Play();
    }

    public void StopSteps()
    {
        foreach (AudioSource source in Sources)
            source.Stop();
    }
}