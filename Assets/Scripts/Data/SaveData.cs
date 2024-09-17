/// <summary>
/// A class that contains all savable data for the Player. <br/>
/// This class is serialized into binary files from the `DataHandler.cs` script.
/// </summary>
[System.Serializable]
public class SaveData
{
    /// <summary>
    /// The Filename given when saving the data, used to dictate information easier when caching data in `DataHandler.cs`!
    /// </summary>
    public string filename;

    /// <summary>
    /// The Name of the Scene / Level that's currently being used (DO NOT INCLUDE NON-LEVEL SCENES!)
    /// </summary>
    public string levelName;
    
    /// <summary>
    /// The amount of times the Player has died, recorded for the end sequence display.
    /// </summary>
    public int deaths;

    /// <summary>
    /// How much progress (0% - 100%) the Player has for this specific Save File.
    /// </summary>
    public float gameProgress;

    /// <summary>
    /// The name of the checkpoint `GameObject`, used to validate if the target checkpoint has already been registered or not.
    /// </summary>
    public string checkpointName;

    /// <summary>
    /// The exact Position values of where the Player was when they reached the latest Checkpoint Instance.
    /// </summary>
    public float[] checkpointPosition;

    /// <summary>
    /// The exact Rotation values of where the Player was when they reached the latest Checkpoint Instance.
    /// </summary>
    public float[] checkpointRotation;

    /// <summary>
    /// The exact Time and Date when the file was first created.
    /// </summary>
    public string creationData;

    /// <summary>
    /// The exact Time and Date when the data was most recently modified.
    /// </summary>
    public string modificationData;
}
