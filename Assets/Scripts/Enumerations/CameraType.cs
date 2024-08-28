/// <summary>
/// An Enumeration that describes what mode the `CameraSystem.cs` script can use.
/// </summary>
public enum CameraType
{
    /// <summary>
    /// Locks the Camera in place, preventing it from moving, rotating or scaling.
    /// </summary>
    Fixed = 0,

    /// <summary>
    /// Follow the `CameraSubject` GameObject with the paramaters provided within `CameraSystem.cs`.
    /// </summary>
    Follow,

    /// <summary>
    /// Similar to Fixed, however the Camera rotates depending on where the `CameraSubject` is within the scene.
    /// </summary>
    Dolly,

    /// <summary>
    /// Allow the `CameraSystem.cs` script to be manipulated by other scripts that choose to do so, mainly for cutscenes.
    /// </summary>
    Scriptable
}