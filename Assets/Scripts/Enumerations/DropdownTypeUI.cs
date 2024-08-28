/// <summary>
/// An Enumeration that dictates the type of Dropdown contents that are applied whenever a `DropdownUI.cs` script is ran. <br/>
/// Used under a `SettingsUI.cs` script context.
/// </summary>
public enum DropdownTypeUI
{
    /// <summary>
    /// Clear out the Dropdown entirely in case another script wants to edit it.
    /// </summary>
    None = 0,

    /// <summary>
    /// Provide your own contents to import into the Dropdown.
    /// </summary>
    Custom,

    /// <summary>
    /// Get the permitted resolutions from the `UnityEngine.Resolution` class when the Dropdown calls it's `Awake()` method.
    /// </summary>
    Resolution,

    /// <summary>
    /// Get all supported modes the game can run under from Unity's `UnityEngine.FullScreenMode` Enumerations.
    /// </summary>
    WindowModes,

    /// <summary>
    /// Get all supported HUD scales the game can use from the `HudScale` Enumeration.
    /// </summary>
    HudScale
}
