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
            
            TitleUI title = FindObjectOfType<TitleUI>();

            if (title != null)
            {
                string combo = title.SubTitleText + " | ";
                if (string.IsNullOrWhiteSpace(title.SubTitleText)) combo = string.Empty;

                title.SubTitle.text = combo + "Main Menu";
            }

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
