/// <summary>
/// An Enumeration that dictates how the `PlayerSystem.cs` script can utilize it's InputDelta Vector2.
/// </summary>
public enum MoveType
{
    /// <summary>
    /// Lock the Player's movement so they're unable to move no matter what input they press.
    /// </summary>
    None = 0,
    
    /// <summary>
    /// Lock the Player's movement only to Left and Right, they cannot move in any other direction.
    /// </summary>
    LockToLeftRight,
    
    /// <summary>
    /// Lock the Player's movement only to Forward and Back, they cannot move in any other direction.
    /// </summary>
    LockToForwardBack,
    
    /// <summary>
    /// Lock the Player to all Two Dimentional Movement (2D) options only, no Three Dimentional (3D) directions are permitted.
    /// </summary>
    TwoDimensionsOnly,

    /// <summary>
    /// Let the Player move in any direction possible with their control scheme.
    /// </summary>
    FreeRoam
}