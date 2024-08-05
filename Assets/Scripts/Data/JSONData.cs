using System.IO;
using UnityEngine;

public class JSONData : MonoBehaviour
{
    [field: SerializeField] private string FileName = "SettingsData.json";
    [field: SerializeField] private string DirectoryName = "MiscellaneousData";
    [field: SerializeField] private bool DefaultOnStartup = false;

    [field: SerializeField] private PlayerSettings DefaultSettings;

    private PlayerSettings lastSavedSettings;

    private string FilePath;
    private string DirectoryPath;

    public PlayerSettings GetDefaultData() => DefaultSettings;
    public PlayerSettings GetCurrentData()
    {
        string fileContents = File.ReadAllText(FilePath);
        return JsonUtility.FromJson<PlayerSettings>(fileContents);
    }

    public void ApplyData(PlayerSettings NewSettings)
    {
        if (NewSettings == lastSavedSettings)
        {
            Debug.Log(name + " | Newer PlayerSettings Instance is exactly the same as the Last Saved Data, skipping...");
            return;
        }

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

            File.Create(FilePath);
            ApplyData(DefaultSettings);

            lastSavedSettings = DefaultSettings;
        }

        if (!DefaultOnStartup || fileDidntExistPrior) return;

        ApplyData(DefaultSettings);
    }
}
