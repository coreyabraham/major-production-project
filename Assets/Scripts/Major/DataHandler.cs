using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
//using UnityEngine.Events;

public class DataHandler : Singleton<DataHandler>
{
    //[Serializable]
    //public class DataEvents
    //{
    //    public UnityEvent DataValidated;
    //    public UnityEvent CreatedFile;
    //    public UnityEvent SavedToFile;
    //    public UnityEvent LoadedFromFile;
    //    public UnityEvent DeletedFile;
    //}

    [field: SerializeField] private string FolderName = "SaveData";
    [field: SerializeField] private string TargetPath = string.Empty;

    [field: Space(2.5f)]
    
    [field: SerializeField] private string ExtensionName = ".dat";
    [field: SerializeField] private string SaveFileName = "SaveFile";

    [field: Space(0.5f)]

    [field: SerializeField] private bool ValidateDataOnStartup = true;

    private const int MaxSaveFiles = 3;

    public int GetMaxSaveFiles() => MaxSaveFiles;
    public string GetFileName(int Index) => GetFilePath() + "/" + SaveFileName + "_" + Index.ToString() + ExtensionName;
    public string GetFilePath() => TargetPath + "/" + FolderName;

    public bool SaveFileData(string filename, SaveData data)
    {
        if (string.IsNullOrWhiteSpace(filename)) return false;
        if (!File.Exists(filename)) return false;

        BinaryFormatter formatter = new();
        FileStream file = File.Create(filename);

        formatter.Serialize(file, data);
        file.Close();

        return true;
    }

    public SaveData LoadSaveFile(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename)) return null;
        if (!File.Exists(filename)) return null;

        BinaryFormatter formatter = new();
        FileStream file = File.Open(filename, FileMode.Open);

        SaveData data = (SaveData)formatter.Deserialize(file);
        file.Close();

        if (data == null)
        {
            Debug.LogWarning(name + " | Failed to Deserialize file: " + filename);
        }

        return data;
    }

    public void CreateSaveFile(string filename)
    {
        print(filename);

        if (string.IsNullOrWhiteSpace(filename)) return;
        if (File.Exists(filename)) return;

        BinaryFormatter formatter = new();
        FileStream file = File.Create(filename);

        SaveData data = new()
        {
            modificationData = DateTime.Now.ToString()
        };

        formatter.Serialize(file, data);
        file.Close();
    }

    public string[] GetSaveFileNames()
    {
        if (!Directory.Exists(GetFilePath())) return new string[0];
        return Directory.GetFiles(GetFilePath());
    }

    public SaveData[] GetSaveFiles()
    {
        if (!Directory.Exists(GetFilePath())) return new SaveData[0];

        string[] files = Directory.GetFiles(GetFilePath());
        SaveData[] data = new SaveData[files.Length];

        for (int i = 0; i < files.Length; i++)
        {
            data[i] = LoadSaveFile(files[i]);
        }

        return data;
    }

    public void ValidateData()
    {
        if (!Directory.Exists(GetFilePath())) Directory.CreateDirectory(GetFilePath());
        for (int i = 0; i < MaxSaveFiles; i++) CreateSaveFile(GetFileName(i));
    }

    protected override void Initialize()
    {
        if (string.IsNullOrWhiteSpace(TargetPath)) TargetPath = Application.persistentDataPath;
        if (!ValidateDataOnStartup) return;

        ValidateData();
    }
}