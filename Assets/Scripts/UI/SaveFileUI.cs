using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class SaveFileUI : MonoBehaviour
{
    public Button Button;
    public Image[] IconImages;

    public TMP_Text NameLabel;
    public TMP_Text DateLabel;

    private SaveData AssignedData;

    [HideInInspector] public int AssignedIndex;

    public SaveData GetData() => AssignedData;
    public void AssignData(SaveData data) => AssignedData = data;
    public void UnassignData() => AssignedData = null;
}
