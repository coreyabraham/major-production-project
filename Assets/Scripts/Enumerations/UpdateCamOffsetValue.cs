/// <summary>
/// [DEPRICATED] <br/>
/// An Enumeration used in the `UpdateCamOffset.cs` script to dictate how the `CameraTarget` value is sent to `CameraSystem.cs`.
/// </summary>
public enum UpdateCamOffsetValue
{
    /// <summary>
    /// Send both the Position and the Rotation.
    /// </summary>
    Both = 0,

    /// <summary>
    /// Only send the Position, ignore the Rotation.
    /// </summary>
    Position,

    /// <summary>
    /// Only send the Rotation, ignore the Position.
    /// </summary>
    Rotation
}