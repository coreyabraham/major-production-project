using TMPro;
using UnityEngine;

public class PlayUI : MonoBehaviour
{
    [field: Header("Templates")]
    [field: SerializeField] private SaveFileUI SaveTemplate;

    [field: Header("Parents")]
    [field: SerializeField] private GameObject SavesParent;
    [field: SerializeField] private GameObject ChosenParent;
    [field: SerializeField] private GameObject ButtonsParent;

    [field: Header("Labels")]
    [field: SerializeField] private TMP_Text SelectLabel;
    [field: SerializeField] private TMP_Text LevelLabel;
    [field: SerializeField] private TMP_Text DeathLabel;

    [field: Header("Miscellaneous")]
    [field: SerializeField] private int StartingLevelIndex;
    [field: SerializeField] private string NonSelectedText;

    [field: Header("Prompter")]
    public PromptUI PromptSystem;
    [field: SerializeField] private PromptDataUI ClearData;

    private SaveFileUI[] CachedFiles;
    private int SaveFileUI_Index = -1;

    private bool Initialized = false;
    
    private bool SaveIndexIsValid() => SaveFileUI_Index > -1 && SaveFileUI_Index < DataHandler.Instance.GetMaxSaveFiles();

    public void PlayButtonPressed()
    {
        if (!SaveIndexIsValid() || CachedFiles[SaveFileUI_Index] == null)
        {
            Debug.LogWarning(name + " | Cannot Play from Save File with Index: " + SaveFileUI_Index.ToString() + "!");
            return;
        }

        DataHandler.Instance.SetCurrentSaveFileIndex((uint)SaveFileUI_Index);
        SaveData targetData = CachedFiles[SaveFileUI_Index].GetData();

        if (string.IsNullOrWhiteSpace(targetData.levelName))
            targetData.levelName = GameSystem.Instance.GetLevelNameWithIndex(StartingLevelIndex);

        DataHandler.Instance.SetCachedData(targetData);
        GameSystem.Instance.RequestLoadScene(targetData.levelName);
    }

    public void ClearButtonPressed()
    {
        if (!SaveIndexIsValid()) return;

        ClearData.PromptFinalized = ClearDataFinished;
        PromptSystem.Begin(ClearData);
    }

    public void ExitButtonPressed()
    {
        if (SaveIndexIsValid()) SaveFileUI_Index = -1;
        if (PromptSystem.PromptActive()) PromptSystem.ForceEnd();
    }

    private void ClearDataFinished(bool result)
    {
        if (!result) return;

        string filename = DataHandler.Instance.GetFileName(SaveFileUI_Index);
        DataHandler.Instance.DestroyFileData(filename);

        CachedFiles[SaveFileUI_Index].DateLabel.text = "Last Modified: ";
        CachedFiles[SaveFileUI_Index].AssignData(DataHandler.Instance.LoadSaveFile(filename));

        OnDisable();
    }

    private void SaveFileSelected(SaveFileUI file)
    {
        if (SaveIndexIsValid() && file == CachedFiles[SaveFileUI_Index])
        {
            SaveFileUI_Index = -1;
            SelectLabel.text = NonSelectedText;

            ChosenParent.SetActive(false);

            return;
        }

        SaveFileUI_Index = file.AssignedIndex;

        SelectLabel.text = "Selected File: " + file.NameLabel.text;

        string levelName = file.GetData().levelName;
        if (string.IsNullOrWhiteSpace(levelName)) levelName = "N / A";

        LevelLabel.text = "Level Name: " + levelName;
        DeathLabel.text = "Deaths: " + file.GetData().deaths.ToString();

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

            clone.AssignedIndex = i;

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

        SaveFileUI_Index = -1;
        SelectLabel.text = NonSelectedText;
    }
}
