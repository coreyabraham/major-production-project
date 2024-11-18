using UnityEditor;
using UnityEngine;

public class ExitUI : MonoBehaviour
{
    [field: Header("Private Externals")]
    [field: SerializeField] private GameObject Main;

    [field: Header("Public References")]
    public PromptUI PromptUI;

    [field: Header("Lists and Arrays")]
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
