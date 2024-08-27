/// <summary>
/// An Enumeration used to dictate how the `SliderUI.cs` script displays it's current value.
/// </summary>
public enum SliderTypeUI
{
    /// <summary>
    /// Do not format the current value, leave it as is.
    /// </summary>
    None = 0,

    /// <summary>
    /// Format the current value as a percentage.
    /// </summary>
    Percentage,

    /// <summary>
    /// Round the current value to a whole number.
    /// </summary>
    WholeNumber
}
