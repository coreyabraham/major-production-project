[System.Serializable]
public class SaveData
{
    /// <summary>
    /// The Name of the Scene / Level that's currently being used (DO NOT INCLUDE NON-LEVEL SCENES!)
    /// </summary>
    public string levelName;
    
    /// <summary>
    /// The Health the Player is currently on before saving, this is used to protect against relogging.
    /// </summary>
    public int health;
    
    /// <summary>
    /// The amount of times the Player has died, recorded for the end sequence display.
    /// </summary>
    public int deaths;

    /// <summary>
    /// How much progress (0% - 100%) the Player has for this specific Save File.
    /// </summary>
    public float gameProgress;

    /// <summary>
    /// The exact Position values of where the Player was when they reached the latest Checkpoint Instance.
    /// </summary>
    public System.Numerics.Vector3 checkpointPosition;

    /// <summary>
    /// The exact Rottaion values of where the Player was when they reached the latest Checkpoint Instance.
    /// </summary>
    public System.Numerics.Vector3 checkpointRotation;
}
