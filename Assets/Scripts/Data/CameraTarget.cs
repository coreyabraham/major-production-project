using UnityEngine;

/// <summary>
/// A Struct that contains Position (Vector3) and Rotation (Quaternion) Data <br/>
/// This Struct is used as a stand-in for Unity's "Transform" system, just without the Scale Vector3. <br/>
/// This Data is mostly utilized in the `CameraSystem.cs` script.
/// </summary>
[System.Serializable]
public struct CameraTarget
{
    /// <summary>
    /// A Vector3 used to dictate object Positions.
    /// </summary>
    public Vector3 position;

    /// <summary>
    /// A Quaternion used to dictate object Rotations.
    /// </summary>
    public Quaternion rotation;
}