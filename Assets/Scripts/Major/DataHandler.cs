using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
using UnityEngine.Events;
using System;

public class DataHandler : MonoBehaviour
{
    [System.Serializable]
    public class DataEvents
    {
        public UnityEvent DataValidated;
        public UnityEvent CreatedFile;
        public UnityEvent SavedToFile;
        public UnityEvent LoadedFromFile;
        public UnityEvent DeletedFile;
    }

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
            creationData = DateTime.Now.ToString()
        };

        formatter.Serialize(file, data);
        file.Close();
    }

    public List<string> GetSaveFileNames()
    {
        if (!Directory.Exists(GetFilePath())) return new();

        string[] files = Directory.GetFiles(GetFilePath());
        List<string> names = new();

        foreach (string file in files) names.Add(file);

        return names;
    }

    public List<SaveData> GetSaveFiles()
    {
        if (!Directory.Exists(GetFilePath())) return new();

        string[] files = Directory.GetFiles(GetFilePath());
        List<SaveData> data = new();

        foreach (string file in files) data.Add(LoadSaveFile(file));

        return data;
    }

    public void ValidateData()
    {
        print(GetFilePath());
        if (!Directory.Exists(GetFilePath())) Directory.CreateDirectory(GetFilePath());

        for (int i = 0; i < MaxSaveFiles; i++)
        {
            print(i);
            CreateSaveFile(GetFileName(i));
        }
    }

    private void Awake()
    {
        if (string.IsNullOrWhiteSpace(TargetPath)) TargetPath = Application.persistentDataPath;
        if (!ValidateDataOnStartup) return;

        ValidateData();
    }
}
