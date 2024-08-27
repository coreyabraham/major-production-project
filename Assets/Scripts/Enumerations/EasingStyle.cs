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
    /// Interpolate the current value to the target value using linear interpolation.
    /// </summary>
    Linear,

    /// <summary>
    /// Interpolate the current value to the target value using sine interpolation (slerp).
    /// </summary>
    Sine,
}