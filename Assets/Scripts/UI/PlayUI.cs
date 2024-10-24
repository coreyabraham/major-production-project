using System;
using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayUI : MonoBehaviour
{
    [field: Header("Miscellaneous")]
    [field: SerializeField] private string[] LevelEntries;

    [field: Header("Parents")]
    [field: SerializeField] private GameObject SavesParent;
    [field: SerializeField] private GameObject ChosenParent;
    [field: SerializeField] private GameObject ButtonsParent;

    [field: Space(5.0f)]

    [field: SerializeField] private GameObject SceneSelector;
    [field: SerializeField] private GameObject ScenesParent;

    [field: Header("Templates")]
    [field: SerializeField] private SaveFileUI SaveTemplate;
    [field: SerializeField] private NavigatorButton SceneTemplate;

    [field: Header("Labels")]
    [field: SerializeField] private TMP_Text SelectLabel;
    [field: SerializeField] private TMP_Text LevelLabel;
    [field: SerializeField] private TMP_Text DeathLabel;
    [field: SerializeField] private TMP_Text ProgressLabel;

    [field: Space(5.0f)]

    [field: SerializeField] private string NonSelectedText = "Selected File: N / A";

    [field: Header("Prompter")]
    [field: SerializeField] private PromptUI PromptSystem;
    [field: SerializeField] private PromptDataUI ClearData;

    private SaveFileUI[] CachedFiles;
    private int SaveFileUI_Index = -1;

    private string[] SceneNames;

    private bool Initialized = false;

    public void TestBtnClicked()
    {
        if (SceneSelector.activeSelf)
        {
            SavesParent.SetActive(true);
            ButtonsParent.SetActive(true);
            SelectLabel.enabled = true;

            SceneSelector.SetActive(false);

            return;
        }

        SavesParent.SetActive(false);
        ChosenParent.SetActive(false);
        ButtonsParent.SetActive(false);
        SelectLabel.enabled = false;

        SceneSelector.SetActive(true);
    }

    public void PlayButtonPressed()
    {
        DataHandler.Instance.SetCurrentSaveFileIndex((uint)SaveFileUI_Index);
        SaveData targetData = CachedFiles[SaveFileUI_Index].GetData();

       if (string.IsNullOrWhiteSpace(targetData.levelName))
       {
            targetData.levelName = LevelEntries[0];
            DataHandler.Instance.SetCachedData(targetData);
            SceneManager.LoadScene(targetData.levelName);
            
            return;
       }
        
        DataHandler.Instance.SetCachedData(targetData);
        SceneManager.LoadScene(targetData.levelName);
    }

    public void ClearButtonPressed()
    {
        ClearData.PromptFinalized = ClearDataFinished;
        PromptSystem.Begin(ClearData);
    }

    private void ClearDataFinished(bool result)
    {
        if (!result) return;

        SaveData clearData = new()
        {
            filename = System.IO.Path.GetFileNameWithoutExtension(DataHandler.Instance.GetFileName(SaveFileUI_Index)),

            checkpointPosition = new float[3],
            checkpointRotation = new float[3],

            modificationData = DateTime.Now.ToString()
        };

        DataHandler.Instance.SaveFileData(DataHandler.Instance.GetFileName(SaveFileUI_Index), clearData);
        CachedFiles[SaveFileUI_Index].AssignData(clearData);

        print("Refresh User Interface Here!");
    }

    private void SceneButtonClicked(int SceneIndex) => SceneManager.LoadScene(SceneIndex, LoadSceneMode.Single);

    private void UpdateSaveFileIndex(SaveFileUI file)
    {
        // TODO: Improve this post-prototype phase!
        for (int i = 0; i < CachedFiles.Length; i++)
        {
            if (CachedFiles[i] != file) continue;

            SaveFileUI_Index = i;
            break;
        }
    }

    private void SaveFileSelected(SaveFileUI file)
    {
        if (SaveFileUI_Index != -1 && file == CachedFiles[SaveFileUI_Index])
        {
            SaveFileUI_Index = -1;
            SelectLabel.text = NonSelectedText;

            ChosenParent.SetActive(false);

            return;
        }

        UpdateSaveFileIndex(file);

        SelectLabel.text = "Selected File: " + file.NameLabel.text;

        string levelName = file.GetData().levelName;
        if (string.IsNullOrWhiteSpace(levelName)) levelName = "N / A";

        LevelLabel.text = "Level Name: " + levelName;
        DeathLabel.text = "Deaths: " + file.GetData().deaths.ToString();
        ProgressLabel.text = "Progress: " + file.GetData().gameProgress.ToString() + "%";

        ChosenParent.SetActive(true);
    }

    private void OnEnable()
    {
        SaveTemplate.gameObject.SetActive(false);
        ChosenParent.SetActive(false);

        if (Initialized) return;

        SaveData[] files = DataHandler.Instance.GetSaveFiles();
        CachedFiles = new SaveFileUI[files.Length];

        for (int i = 0; i < files.Length; i++)
        {
            SaveFileUI clone = Instantiate(SaveTemplate);

            clone.name = "SaveFile_" + (i + 1).ToString();
            clone.transform.SetParent(SavesParent.transform, false);
            clone.gameObject.SetActive(true);

            clone.NameLabel.text = "Save File: " + (i + 1).ToString();
            clone.DateLabel.text = "Last Modified: " + files[i].modificationData;

            clone.AssignData(files[i]);
            clone.Button.onClick.AddListener(() => SaveFileSelected(clone));

            CachedFiles[i] = clone;
        }

        int sceneCount = SceneManager.sceneCountInBuildSettings;
        SceneNames = new string[sceneCount];

        for (int i = 0; i < sceneCount; i++)
        {
            int index = i;

            SceneNames[index] = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(index));

            NavigatorButton clone = Instantiate(SceneTemplate);
            clone.transform.SetParent(ScenesParent.transform, false);

            clone.name = SceneNames[index];
            clone.Text.text = "Scene " + (i + 1).ToString() + ": " + clone.name;

            clone.gameObject.SetActive(true);
            clone.Button.onClick.AddListener(() => SceneButtonClicked(index));
        }

        Initialized = true;
    }

    private void OnDisable()
    {
        ChosenParent.SetActive(false);

        LevelLabel.text = "Level Name: null";
        DeathLabel.text = "Deaths: 0";
        ProgressLabel.text = "Progress: 0%";

        SaveFileUI_Index = -1;
        SelectLabel.text = NonSelectedText;
    }
}
