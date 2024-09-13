using UnityEditor;
using UnityEngine;

public class ExitUI : MonoBehaviour
{
    [field: SerializeField] private GameObject Main;
    public PromptUI PromptUI;

    [field: Space(5.0f)]

    [field: SerializeField] private PromptDataUI PromptData;

    private void PromptFinished(bool result)
    {
        if (!result)
        {
            Main.SetActive(true);
            gameObject.SetActive(false);
            return;
        }

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }

    private void OnEnable()
    {
        PromptData.PromptFinalized = PromptFinished;
        PromptUI.Begin(PromptData);
    }
}
