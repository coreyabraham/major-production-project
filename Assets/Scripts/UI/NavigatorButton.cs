using TMPro;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class NavigatorButton : MonoBehaviour
{
    public GameObject TargetFrame;

    [field: Space(5.0f)]
    
    public Button Button;
    public TMP_Text Text;

    [field: Space(5.0f)]

    public UnityEvent ClickedEvent;
}
