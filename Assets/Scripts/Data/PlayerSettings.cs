/// <summary>
/// A Class for the Player's Audio, Graphics, Visual and Input Settings used during gameplay. <br/>
/// This data is converted into the Json file format and kept under the Player's "PersistantFilePath" location by the `JsonData.cs` script.
/// </summary>
[System.Serializable]
public class PlayerSettings
{
    // Visual Representation of Audio: 0% - 100%, internally all volumes are registered as 0 - 1 in decimals.
    #region Audio
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

    #region Graphics
    /// <summary>
    /// The Resolution the game will render at during gameplay.
    /// </summary>
    public string Resolution;
    
    /// <summary>
    /// The Quality Setting that the game's visuals will run under, is adjustable via "Project Settings"!
    /// </summary>
    public string Quality;

    /// <summary>
    /// The Fullscreen Mode the current session will run in.
    /// </summary>
    public string WindowMode;

    /// <summary>
    /// The Scale of ALL User Interfaces in the game. <br/>
    /// This includes gameplay specific UI elements as well.
    /// </summary>
    public string HudScale;

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

    #region Visuals
    /// <summary>
    /// The Contrast the game will use to make things pop more for the sake of easier sightings.
    /// </summary>
    public float Contrast;

    /// <summary>
    /// The Brightness the game will use to make things easier to see for players.
    /// </summary>
    public float Brightness;

    /// <summary>
    /// The filter type the game will use to support those who have colour-based imparements.
    /// </summary>
    public string ColourAccessibility;
    #endregion

    #region Input
    /// <summary>
    /// [ANALOG] The 2D Vector Action that's responsible for housing all movement bindings.
    /// </summary>
    public string[] AnalogMoveAction;

    /// <summary>
    /// [DIGITAL] The 2D Vector Action that's responsible for housing all movement bindings.
    /// </summary>
    public string[] DigitalMoveAction;

    /// <summary>
    /// The Jump Action that's responsible for housing the buttons necessary to activate a jump.
    /// </summary>
    public string[] JumpAction;

    /// <summary>
    /// The Scurry Action that's responsible for housing the buttons necessary to activate a scurry.
    /// </summary>
    public string[] ScurryAction;

    /// <summary>
    /// The Climb Action that's responsible for housing the buttons necessary to activate a pipe climb.
    /// </summary>
    public string[] ClimbAction;

    /// <summary>
    /// The Interact Action that's responsible for housing the buttons necessary to interact with nearby objects.
    /// </summary>
    public string[] InteractAction;

    /// <summary>
    /// The Pause Action that's responsible for housing the buttons necessary to toggle the Pause Menu.
    /// </summary>
    public string[] PauseAction;
    #endregion
}