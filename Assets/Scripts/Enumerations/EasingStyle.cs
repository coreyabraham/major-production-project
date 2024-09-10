/// <summary>
/// An Enumeration that dictates how Easing should be handled with scripts that choose to interpolate. <br/>
/// Mainly utilized within the `CameraSystem.cs` script.
/// </summary>
public enum EasingStyle
{
    /// <summary>
    /// Do not interpolate, instead simply move the current value to the target value.
    /// </summary>
    None = 0,

    /// <summary>
    /// Move from Point A to Point B with no interpolation.
    /// </summary>
    Basic,

    /// <summary>
    /// Interpolate the current value to the target value using linear interpolation.
    /// </summary>
    Lerp,

    /// <summary>
    /// Interpolate the current value to the target value using spherical interpolation (slerp).
    /// </summary>
    Slerp,
}