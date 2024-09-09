/// <summary>
/// An Enumeration that dictates what Playback option the User can pick in the `AudioHandler.cs` system.
/// </summary>
public enum AudioPlaybackType
{
    Play = 0,
    PlayDelayed,
    PlayOneShot,
    PlayScheduled,

    Pause,
    Stop
}