using UnityEngine;

/// <summary>
/// Footsteps should not be handled like this, but for the sake of keeping footstep sounds in, it can stay for now.
/// </summary>
/// 
public class SoundInfo
{
    [Tooltip("Volume to play the sound at (Percent)")]
    [Range(0.0f, 100.0f)]
    public float volumePercent = 100.0f;

    [Header("Pitch")]
    [Range(-3.0f, 3.0f)]
    public float min = 1;
    [Range(-3.0f, 3.0f)]
    public float max = 1;
}
public class RatFootstepSound : MonoBehaviour
{
    [field: SerializeField] AudioSource footstepsSound;
    PlayerSystem playSys;


    [field: SerializeField] public SoundInfo ShovelInDigSpot { get; private set; }

    void Update()
    {
        if (!(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) || !playSys.IsPlayerGrounded())
        {
            if (footstepsSound.enabled) { footstepsSound.enabled = false; }
            return; 
        }
        footstepsSound.enabled = true;
    }

    private void Play(Vector3 position, SoundInfo soundInfo)
    {
        playSys = footstepsSound.gameObject.GetComponent<PlayerSystem>();
        footstepsSound.enabled = false;

       // SoundInfo.pitch = UnityEngine.Random.Range(soundInfo.min, soundInfo.max);
        //soundInfo.volume = soundInfo.volumePercent / 100.0f;
    }
}