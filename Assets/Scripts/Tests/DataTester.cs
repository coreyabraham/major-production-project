using System;
using UnityEngine;

public class DataTester : MonoBehaviour
{
    [System.Serializable]
    public enum DATA_TestType
    {
        None = 0,
        GetSaveFiles,
        CreateSaveFile,
        LoadSaveFile,
        SaveFileData
    }

    [field: SerializeField] private DATA_TestType testingType = DATA_TestType.CreateSaveFile;

    [field: SerializeField] private int SaveFileIndex = 0;
    [field: SerializeField] private bool TestAllSaveFiles = false;

    private void Start()
    {
        switch (testingType)
        {
            case DATA_TestType.GetSaveFiles:
                {
                    SaveData[] data = DataHandler.Instance.GetSaveFiles();
                    foreach (SaveData file in data)
                    {
                        print(file.modificationData);
                    }
                }
                break;

            case DATA_TestType.CreateSaveFile: 
                {
                    if (TestAllSaveFiles)
                    {
                        string[] data = DataHandler.Instance.GetSaveFileNames();
                        foreach (string file in data)
                        {
                            DataHandler.Instance.CreateSaveFile(file);
                        }

                        return;
                    }

                    DataHandler.Instance.CreateSaveFile(DataHandler.Instance.GetFileName(SaveFileIndex));
                } 
                break;

            case DATA_TestType.LoadSaveFile:
                {
                    if (TestAllSaveFiles)
                    {
                        string[] data = DataHandler.Instance.GetSaveFileNames();
                        foreach (string file in data)
                        {
                            DataHandler.Instance.LoadSaveFile(file);
                        }

                        return;
                    }

                    print(DataHandler.Instance.LoadSaveFile(DataHandler.Instance.GetFileName(SaveFileIndex)));
                }
                break;

            case DATA_TestType.SaveFileData:
                {
                    string filename;

                    SaveData SD = new()
                    {
                        modificationData = DateTime.Now.ToString()
                    };

                    if (TestAllSaveFiles)
                    {
                        string[] data = DataHandler.Instance.GetSaveFileNames();
                        foreach (string file in data)
                        {
                            DataHandler.Instance.SaveFileData(file, SD);
                        }

                        return;
                    }

                    filename = DataHandler.Instance.GetFileName(SaveFileIndex);
                    DataHandler.Instance.SaveFileData(filename, SD);
                }
                break;

            default: break;
        }
    }
}
