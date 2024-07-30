using TMPro;
using UnityEngine;

public class PlayUI : MonoBehaviour
{
    [field: SerializeField] private GameObject SavesParent;
    [field: SerializeField] private GameObject ChosenParent;

    [field: Space(5.0f)]

    [field: SerializeField] private SaveFileUI Template;
    [field: SerializeField] private TMP_Text SelectLabel;

    [field: Space(5.0f)]

    [field: SerializeField] private TMP_Text LevelLabel;
    [field: SerializeField] private TMP_Text DeathLabel;
    [field: SerializeField] private TMP_Text ProgressLabel;

    private SaveFileUI[] CachedFiles;
    private int SaveFileUI_Index = -1;

    private bool Initialized = false;

    public void PlayButtonPressed()
    {
        print("Play with File: " + CachedFiles[SaveFileUI_Index].name);
    }

    public void ClearButtonPressed()
    {
        print("Clear Data from File: " + CachedFiles[SaveFileUI_Index].name);
    }

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
            SelectLabel.text = "Selected File: null";

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
        Template.gameObject.SetActive(false);
        ChosenParent.SetActive(false);

        if (Initialized) return;

        SaveData[] files = DataHandler.Instance.GetSaveFiles();
        CachedFiles = new SaveFileUI[files.Length];

        for (int i = 0; i < files.Length; i++)
        {
            SaveFileUI clone = Instantiate(Template);

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
        SelectLabel.text = "Selected File: null";
    }
}
