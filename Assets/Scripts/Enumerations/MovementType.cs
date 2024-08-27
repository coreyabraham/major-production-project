/// <summary>
/// An Enumeration that dictates how the `PlayerSystem.cs` script can utilize it's InputDelta Vector2.
/// </summary>
public enum MovementType
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
    /// Let the Player move in any direction possible with their control scheme.
    /// </summary>
    FreeRoam
}