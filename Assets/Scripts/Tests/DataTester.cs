using System;
using System.Collections.Generic;
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

    [field: SerializeField] private DataHandler dataHandler;
    [field: SerializeField] private DATA_TestType testingType = DATA_TestType.CreateSaveFile;

    [field: SerializeField] private int SaveFileIndex = 0;
    [field: SerializeField] private bool TestAllSaveFiles = false;

    private void Start()
    {
        switch (testingType)
        {
            case DATA_TestType.GetSaveFiles:
                {
                    List<SaveData> data = dataHandler.GetSaveFiles();
                    foreach (SaveData file in data)
                    {
                        print(file.creationData);
                    }
                }
                break;

            case DATA_TestType.CreateSaveFile: 
                {
                    if (TestAllSaveFiles)
                    {
                        List<string> data = dataHandler.GetSaveFileNames();
                        foreach (string file in data)
                        {
                            dataHandler.CreateSaveFile(file);
                        }

                        return;
                    }

                    dataHandler.CreateSaveFile(dataHandler.GetFileName(SaveFileIndex));
                } 
                break;

            case DATA_TestType.LoadSaveFile:
                {
                    if (TestAllSaveFiles)
                    {
                        List<string> data = dataHandler.GetSaveFileNames();
                        foreach (string file in data)
                        {
                            dataHandler.LoadSaveFile(file);
                        }

                        return;
                    }

                    print(dataHandler.LoadSaveFile(dataHandler.GetFileName(SaveFileIndex)));
                }
                break;

            case DATA_TestType.SaveFileData:
                {
                    string filename;

                    SaveData SD = new()
                    {
                        creationData = DateTime.Now.ToString()
                    };

                    if (TestAllSaveFiles)
                    {
                        List<string> data = dataHandler.GetSaveFileNames();
                        foreach (string file in data)
                        {
                            dataHandler.SaveFileData(file, SD);
                        }

                        return;
                    }

                    filename = dataHandler.GetFileName(SaveFileIndex);
                    dataHandler.SaveFileData(filename, SD);
                }
                break;

            default: break;
        }
    }
}
