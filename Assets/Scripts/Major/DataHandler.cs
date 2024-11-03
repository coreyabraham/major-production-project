using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

public class DataHandler : Singleton<DataHandler>
{
    [field: SerializeField] private string FolderName = "SaveData";
    [field: SerializeField] private string TargetPath = string.Empty;

    [field: Space(2.5f)]
    
    [field: SerializeField] private string ExtensionName = ".dat";
    [field: SerializeField] private string SaveFileName = "SaveFile";

    [field: Space(0.5f)]

    [field: SerializeField] private bool ValidateDataOnStartup = true;
    [field: SerializeField] private bool CacheDataOnStartup = false;
    [field: SerializeField] private bool ClearDataOnStartup = false;
    [field: SerializeField] private bool IgnoreSaveRequests = false;

    private const int MaxSaveFiles = 3;
    private int CurrentSaveFileIndex = -1;

    private SaveData CachedData;

    public int GetCurrentSaveFileIndex() => CurrentSaveFileIndex;
    public void SetCurrentSaveFileIndex(uint Value) => CurrentSaveFileIndex = (int)Value;

    public SaveData RefreshCachedData(string Filename)
    {
        CachedData = LoadSaveFile(Filename);
        return CachedData;
    }
    public SaveData RefreshCachedData()
    {
        CachedData = LoadSaveFile();
        return CachedData;
    }

    public SaveData GetCachedData() => CachedData;
    public void SetCachedData(SaveData Data) => CachedData = Data;
    public bool SaveCachedDataToFile(string Filename) => SaveFileData(Filename, CachedData);
    public bool SaveCachedDataToFile() => SaveFileData(GetFileName(), CachedData);

    public int GetMaxSaveFiles() => MaxSaveFiles;
    public string GetFileName(int Index) => GetFilePath() + "/" + SaveFileName + "_" + Index.ToString() + ExtensionName;
    public string GetFilePath() => TargetPath + "/" + FolderName;

    public Vector3 ConvertFloatArrayToVector3(float[] array)
    {
        if (array.Length != 3)
        {
            Debug.LogWarning(name + " | Could not read provided Float Arrray for Array to Vector3 Conversion, the provided Array didn't include exactly 3 float values!");
            return Vector3.zero;
        }

        Vector3 vector = new()
        {
            x = float.MaxValue,
            y = float.MaxValue,
            z = float.MaxValue
        };

        vector.x = array[0];
        vector.y = array[1];
        vector.z = array[2];

        if (vector.x == float.MaxValue)
        {
            Debug.LogWarning(name + " | Could not convert provided Float Array (float[] array) into a Vector3 instance, returning empty Vector3...");
            return Vector3.zero;
        }

        if (vector.y == float.MaxValue)
        {
            Debug.LogWarning(name + " | Could not convert provided Float Array (float[] array) into a Vector3 instance, returning empty Vector3...");
            return Vector3.zero;
        }

        if (vector.z == float.MaxValue)
        {
            Debug.LogWarning(name + " | Could not convert provided Float Array (float[] array) into a Vector3 instance, returning empty Vector3...");
            return Vector3.zero;
        }

        return vector;
    }

    public float[] ConvertVector3ToFloatArray(Vector3 vector)
    {
        float[] floats = new float[3]
        {
            float.MaxValue,
            float.MaxValue,
            float.MaxValue
        };

        floats[0] = vector.x;
        floats[1] = vector.y;
        floats[2] = vector.z;

        for (int i = 0; i < floats.Length; i++)
        {
            if (floats[i] != float.MaxValue) continue;
            
            Debug.LogWarning(name + " | Could not convert provided Vector3 Instance (Vector3 vector) into a Float Array (float[] array), returning empty float array...");
            return new float[3];
        }

        return floats;
    }

    public string GetFileName()
    {
        if (CurrentSaveFileIndex == -1)
        {
            Debug.LogWarning(name + " | CurrentSaveFileIndex was set to -1 due to it not being set prior, setting to 0 for debugging reasons!");
            CurrentSaveFileIndex = 0;
        }

        return GetFileName(CurrentSaveFileIndex);
    }

    public int GetFileIndexWithName(string Filename)
    {
        string[] names = GetSaveFileNames();
        int found = -1;

        for (int i = 0; i < names.Length; i++)
        {
            if (names[i] != Filename) continue;
            string[] contents = names[i].Split('_');

            foreach (string str in contents)
            {
                if (!int.TryParse(str, out found)) continue;
                break;
            }
        }

        if (found == -1) Debug.LogWarning(name + " | Could not find Integer Index with Name: " + Filename + "! Please make sure it exists before calling this method!");

        return found;
    }

    public bool DestroyFileData(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
        {
            Debug.LogWarning(name + " | No Filename was provided! Please provide a Filename when Deleting File Data!");
            return false;
        }

        if (!File.Exists(filename))
        {
            Debug.LogWarning(name + " | File with Filename: " + filename + " doesn't exist! Please create it first before attempting to delete it!");
            return false;
        }

        SaveData ClearData = new()
        {
            filename = filename,

            checkpointPosition = new float[3],
            checkpointRotation = new float[3],

            creationData = DateTime.Now.ToString(),
            modificationData = string.Empty
        };

        return SaveFileData(filename, ClearData);
    }

    public bool SaveFileData(string filename, SaveData data)
    {
        if (string.IsNullOrWhiteSpace(filename))
        {
            Debug.LogWarning(name + " | No Filename was provided! Please provide a Filename when Saving File Data!");
            return false;
        }

        if (!File.Exists(filename))
        {
            Debug.LogWarning(name + " | File with Filename: " + filename + " doesn't exist! Please create it first before attempting to save to it!");
            return false;
        }

        if (IgnoreSaveRequests)
        {
            Debug.LogWarning(name = " | `IgnoreSaveRequests` is false! Save requests are ignored as such!");
            return false;
        }

        data.modificationData = DateTime.Now.ToString();

        BinaryFormatter formatter = new();
        FileStream file = File.Create(filename);

        formatter.Serialize(file, data);
        file.Close();

        return true;
    }

    public bool SaveFileData() => SaveFileData(GetFileName(), CachedData);

    public SaveData LoadSaveFile(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
        {
            Debug.LogWarning(name + " | Could not retrive `SaveData` struct due to a filename not being provided, be sure your string has actual contents within it before requesting to Load Data!");
            return null;
        }

        if (!File.Exists(filename))
        {
            Debug.LogWarning(name + " | File with Filename: " + filename + " doesn't exist! Please create it first before attempting to load from it!");
            return null;
        }

        BinaryFormatter formatter = new();
        FileStream file = File.Open(filename, FileMode.Open);

        SaveData data = (SaveData)formatter.Deserialize(file);
        file.Close();

        if (data == null) Debug.LogWarning(name + " | Failed to Deserialize file: " + filename + ", nothing will be returned as a result.");

        return data;
    }

    public SaveData LoadSaveFile() => LoadSaveFile(GetFileName());

    public void CreateSaveFile(string filename, bool skipExistanceWarning = false)
    {
        if (string.IsNullOrWhiteSpace(filename))
        {
            Debug.LogWarning(name + " | No Filename was provided! Please provide a Filename when Creating a File!");
            return;
        }

        if (File.Exists(filename))
        {
            if (!skipExistanceWarning) Debug.LogWarning(name + " | File with Filename: " + filename + " already exists within Directory: " + GetFilePath() + "! You cannot create a File with that Filename as a result.");
            return;
        }

        BinaryFormatter formatter = new();
        FileStream file = File.Create(filename);

        SaveData data = new()
        {
            filename = Path.GetFileNameWithoutExtension(filename),

            checkpointPosition = new float[3],
            checkpointRotation = new float[3],
            
            creationData = DateTime.Now.ToString()
        };

        formatter.Serialize(file, data);
        file.Close();
    }

    public string[] GetSaveFileNames()
    {
        if (!Directory.Exists(GetFilePath()))
        {
            Debug.LogWarning(name + " | Directory: " + GetFilePath() + " does not seem to exist right now... please make sure it exists first before getting ALL save file names!");
            return new string[0];
        }

        return Directory.GetFiles(GetFilePath());
    }

    public SaveData[] GetSaveFiles()
    {
        if (!Directory.Exists(GetFilePath()))
        {
            Debug.LogWarning(name + " | Directory: " + GetFilePath() + " does not seem to exist right now... please make sure it exists first before getting ALL save files!");
            return new SaveData[0];
        }

        string[] files = Directory.GetFiles(GetFilePath());
        SaveData[] data = new SaveData[files.Length];

        for (int i = 0; i < files.Length; i++) data[i] = LoadSaveFile(files[i]);

        return data;
    }

    private void ValidateData()
    {
        if (!Directory.Exists(GetFilePath())) 
            Directory.CreateDirectory(GetFilePath());

        for (int i = 0; i < MaxSaveFiles; i++)
        {
            string filename = GetFileName(i);
            CreateSaveFile(filename, true);
        }
    }

    private void Start()
    {
        bool RunningInEditor = false;

#if UNITY_EDITOR
        RunningInEditor = true;
#endif

        if (RunningInEditor || !IgnoreSaveRequests) return;
        IgnoreSaveRequests = false;
    }

    protected override void Initialize()
    {
        if (string.IsNullOrWhiteSpace(TargetPath)) TargetPath = Application.persistentDataPath;

        if (ClearDataOnStartup)
        {
            for (int i = 0; i < MaxSaveFiles; i++) 
                DestroyFileData(GetFileName(i));
        }

        if (CacheDataOnStartup) RefreshCachedData();
        if (ValidateDataOnStartup) ValidateData();
    }
}