using TMPro;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class NavigatorButton : MonoBehaviour
{
    [field: Header("Menus Exclusive")]
    public GameObject TargetFrame;

    [field: Header("Settings")]
    [field: SerializeField] private string Text;

    [HideInInspector] public Button Button;
    [HideInInspector] public TMP_Text Label;

    [HideInInspector] public string DefaultText;

    [field: Header("Events")]
    public UnityEvent ClickedEvent;

    private void Awake()
    {
        Button = GetComponentInChildren<Button>();
        Label = GetComponentInChildren<TMP_Text>();

        ClickedEvent ??= new();

        Button.onClick.AddListener(() => ClickedEvent?.Invoke());

        bool result = string.IsNullOrWhiteSpace(Text);
        DefaultText = !result ? Text : Label.text;

        if (result) return;
        Label.text = Text;
    }
}
