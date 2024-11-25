using TMPro;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class NavigatorButton : MonoBehaviour
{
    [field: Header("Externals")]
    public GameObject TargetFrame;

    [field: Header("Assets")]
    public Button Button;
    public TMP_Text Text;

    [field: Header("Event(s) and Array(s)")]
    public UnityEvent ClickedEvent;

    private void Awake()
    {
        if (!Button) return;
        Button.onClick.AddListener(() => ClickedEvent?.Invoke());
    }
}