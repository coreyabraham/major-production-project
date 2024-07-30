using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class SaveFileUI : MonoBehaviour
{
    public Button Button;
    public Image IconImage;

    public TMP_Text NameLabel;
    public TMP_Text DateLabel;

    private SaveData AssignedData;

    public SaveData GetData() => AssignedData;
    public void AssignData(SaveData data) => AssignedData = data;
    public void UnassignData() => AssignedData = null;
}
