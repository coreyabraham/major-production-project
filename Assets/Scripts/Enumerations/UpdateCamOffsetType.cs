/// <summary>
/// [DEPRICATED] <br/>
/// An Enumeration that dictates how the `UpdateCamOffset.cs` script changes the `Offset` `CameraTarget` in `CameraSystem.cs`.
/// </summary>
public enum UpdateCamOffsetType
{
    /// <summary>
    /// Override the `Offset` entirely.
    /// </summary>
    Target = 0,

    /// <summary>
    /// Revert the `Offset` back to it's runtime default.
    /// </summary>
    Default,
    
    /// <summary>
    /// Use the previously used `Offset` from `CameraSystem.cs`.
    /// </summary>
    Previous
}