using UnityEngine;

[System.Serializable]
public class PlayerSettings
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
    public int Resolution;
    
    /// <summary>
    /// The Quality Setting that the game's visuals will run under, is adjustable via "Project Settings"!
    /// </summary>
    public int Quality;

    /// <summary>
    /// An Enumeration (int int form) to dictate what Fullscreen Mode the current session will run in. <br/>
    /// [ Example: if `fullscreen` is equal to zero, then it's equivalent to: FullScreenMode.Windowed ]
    /// </summary>
    public int Fullscreen;

    /// <summary>
    /// The Scale of ALL User Interfaces in the game. <br/>
    /// This includes gameplay specific UI elements as well.
    /// [ Example: if `hudScale` is equal to one, then it's equivalent to: HudScaleUI.1x ]
    /// </summary>
    public int HudScale;

    /// <summary>
    /// The target amount of Frames Per Second the game will try to reach during gameplay. <br/>
    /// THIS WILL NOT BE READ IF VSYNC IS ENABLED!
    /// </summary>
    public int FramesPerSecond;

    /// <summary>
    /// Cap the Framerate to your Monitor's "Refresh Rate" and yield GPU rendering per frame to prevent screen-tearing <br/>
    /// THIS WILL IGNORE YOUR FRAMES PER SECOND VALUE UPON BEING USED!
    /// </summary>
    public bool UseVsync;
    #endregion
}