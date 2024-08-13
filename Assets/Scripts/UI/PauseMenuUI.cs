using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    [field: SerializeField] private GameObject Frame;

    private bool Toggled = false;

    private void ToggleUI()
    {
        Toggled = !Toggled;
        Frame.SetActive(Toggled);
        Time.timeScale = (Toggled) ? 0.0f : 1.0f;
    }

    private void Awake()
    {
        Toggled = Frame.activeSelf;
        //InputHandler.Instance.Events.Pausing.AddListener(ToggleUI);
    }
}
