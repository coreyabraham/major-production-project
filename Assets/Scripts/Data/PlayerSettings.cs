using UnityEngine;

[System.Serializable]
public struct PlayerSettings
{
    // Visual Representation of Audio: 0% - 100%, internally all volumes are registered as 0 - 1 in decimals.
    #region "Audio"
    /// <summary>
    /// The Volume for every "Sound Source" Component in the scene, sways Music and Sound Volumes as well.
    /// </summary>
    public float MasterVolume;

    /// <summary>
    /// The Volume for ALL Music based "Sound Source" Components created.
    /// </summary>
    public float MusicVolume;

    /// <summary>
    /// The Volume for ALL Sound based "Sound Source" Components created.
    /// </summary>
    public float SoundVolume;
    #endregion

    #region "Graphics"
    /// <summary>
    /// The Resolution the game will render at during gameplay.
    /// </summary>
    public int resolution;
    
    public int quality;

    /// <summary>
    /// An Enumeration to dictate what Fullscreen Mode the current session will run in. <br/>
    /// [ Example: Screen.fullScreenMode = FullScreenMode.Windowed; ]
    /// </summary>
    public FullScreenMode fullscreen;

    /// <summary>
    /// The Scale of ALL User Interfaces in the game. <br/>
    /// This includes gameplay specific UI elements as well.
    /// </summary>
    public HudScaleUI hudScale;

    /// <summary>
    /// The target amount of Frames Per Second the game will try to reach during gameplay. <br/>
    /// THIS WILL NOT BE READ IF VSYNC IS ENABLED!
    /// </summary>
    public float framesPerSecond;

    /// <summary>
    /// Cap the Framerate to your Monitor's "Refresh Rate" and yield GPU rendering per frame to prevent screen-tearing <br/>
    /// THIS WILL IGNORE YOUR FRAMES PER SECOND VALUE UPON BEING USED!
    /// </summary>
    public bool useVsync;
    #endregion
}