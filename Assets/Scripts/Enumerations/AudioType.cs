/// <summary>
/// An Enumeration that dictates what Audio Mixer Group will be utilized. <br/>
/// Mainly centered around the `AudioHandler.cs` script.
/// </summary>
public enum AudioType
{
    /// <summary>
    /// Do not feed the current AudioSource into an Audio Mixer Group.
    /// </summary>
    None = 0,

    /// <summary>
    /// Use the Master Audio Mixer Group.
    /// </summary>
    Master,
    
    /// <summary>
    /// Use the Music Audio Mixer Group.
    /// </summary>
    Music,
    
    /// <summary>
    /// Use the Sound Audio Mixer Group.
    /// </summary>
    Sound
}