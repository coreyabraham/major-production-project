using TMPro;
using UnityEngine;

public class PlayUI : MonoBehaviour
{
    [field: Header("Miscellaneous")]
    [field: SerializeField] private int StartingLevelIndex;

    [field: Header("Parents")]
    [field: SerializeField] private GameObject SavesParent;
    [field: SerializeField] private GameObject ChosenParent;
    [field: SerializeField] private GameObject ButtonsParent;

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
    public PromptUI PromptSystem;
    [field: SerializeField] private PromptDataUI ClearData;

    private SaveFileUI[] CachedFiles;
    private int SaveFileUI_Index = -1;

    private bool Initialized = false;

    public void PlayButtonPressed()
    {
        if (SaveFileUI_Index < 0 || SaveFileUI_Index > CachedFiles.Length || CachedFiles[SaveFileUI_Index] == null)
        {
            Debug.LogWarning(name + " | Cannot Play from Save File with Index: " + SaveFileUI_Index.ToString() + "!");
            return;
        }

        DataHandler.Instance.SetCurrentSaveFileIndex((uint)SaveFileUI_Index);
        SaveData targetData = CachedFiles[SaveFileUI_Index].GetData();

       if (string.IsNullOrWhiteSpace(targetData.levelName))
            targetData.levelName = GameSystem.Instance.GetLevelName(StartingLevelIndex);

        DataHandler.Instance.SetCachedData(targetData);
        GameSystem.Instance.RequestLoadScene(targetData.levelName);
    }

    public void ClearButtonPressed()
    {
        ClearData.PromptFinalized = ClearDataFinished;
        PromptSystem.Begin(ClearData);
    }

    private void ClearDataFinished(bool result)
    {
        if (!result) return;

        string filename = DataHandler.Instance.GetFileName(SaveFileUI_Index);

        DataHandler.Instance.DestroyFileData(filename);
        CachedFiles[SaveFileUI_Index].AssignData(DataHandler.Instance.LoadSaveFile(filename));

        OnDisable();
    }

    private void SceneButtonClicked(int SceneIndex) => GameSystem.Instance.RequestLoadScene(GameSystem.Instance.GetLevelName(SceneIndex));

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
