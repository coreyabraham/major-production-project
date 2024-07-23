using System.Collections.Generic;
using UnityEngine;

public class DataTester : MonoBehaviour
{
    [System.Serializable]
    public enum DATA_TestType
    {
        GetSaveFiles = 0,
        CreateSaveFile,
        LoadSaveFile,
        SaveFileData
    }

    [field: SerializeField] private DataHandler dataHandler;
    [field: SerializeField] private DATA_TestType testingType = DATA_TestType.CreateSaveFile;
    [field: SerializeField] private int SaveFileIndex = 0;

    private void Start()
    {
        switch (testingType)
        {
            case DATA_TestType.GetSaveFiles:
                {
                    List<SaveData> data = dataHandler.GetSaveFiles();
                    foreach (SaveData file in data)
                    {
                        print(file.ToString());
                    }
                }
                break;

            case DATA_TestType.CreateSaveFile: 
                {
                    //dataHandler.CreateSaveFile(dataHandler.GetFileName(0));
                } 
                break;

            case DATA_TestType.LoadSaveFile:
                {
                    print(dataHandler.LoadSaveFile(dataHandler.GetFileName(SaveFileIndex)));
                }
                break;

            case DATA_TestType.SaveFileData:
                {
                    string filename = dataHandler.GetFileName(SaveFileIndex);
                    dataHandler.SaveFileData(filename, dataHandler.LoadSaveFile(filename));
                }
                break;
        }
    }
}
