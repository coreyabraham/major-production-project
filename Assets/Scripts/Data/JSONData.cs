using System.IO;

using UnityEngine;
using UnityEngine.Events;

public class JSONData : MonoBehaviour
{
    [field: SerializeField] private string FileName = "SettingsData.json";
    [field: SerializeField] private string DirectoryName = "MiscellaneousData";
    [field: SerializeField] private bool DefaultOnStartup = false;

    [field: SerializeField] private PlayerSettings DefaultSettings;

    public UnityEvent<PlayerSettings> Initialized;

    private PlayerSettings lastSavedSettings;

    private string FilePath;
    private string DirectoryPath;

    public PlayerSettings GetDefaultData() => DefaultSettings;
    public PlayerSettings GetCurrentData()
    {
        string fileContents = File.ReadAllText(FilePath);
        return JsonUtility.FromJson<PlayerSettings>(fileContents);
    }

    public void ApplyData(PlayerSettings NewSettings, bool SetDefault = false)
    {
        if (NewSettings == lastSavedSettings)
        {
            Debug.Log(name + " | Newer PlayerSettings Instance is exactly the same as the Last Saved Data, skipping...");
            return;
        }

        if (string.IsNullOrWhiteSpace(NewSettings.Resolution)) NewSettings.Resolution = Screen.currentResolution.width.ToString() + "x" + Screen.currentResolution.height;

        string data = JsonUtility.ToJson(NewSettings);
        File.WriteAllText(FilePath, data);

        lastSavedSettings = NewSettings;
    }

    private void Awake()
    {
        DirectoryPath = Application.persistentDataPath + "/" + DirectoryName;
        FilePath = DirectoryPath + "/" + FileName;
        
        if (!Directory.Exists(DirectoryPath)) Directory.CreateDirectory(DirectoryPath);

        bool fileDidntExistPrior = false;

        if (!File.Exists(FilePath))
        {
            fileDidntExistPrior = true;
            ApplyData(DefaultSettings, true);
            lastSavedSettings = DefaultSettings;
        }

        if (DefaultOnStartup && fileDidntExistPrior) ApplyData(DefaultSettings, true);

        Initialized?.Invoke(GetCurrentData());
    }
}
