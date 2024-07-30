using TMPro;

using UnityEngine;

public class PromptUI : Singleton<PromptUI>
{
    [field: SerializeField] private GameObject Prompt;

    [field: Space(10.0f)]

    [field: SerializeField] private TMP_Text TitleLabel;
    [field: SerializeField] private TMP_Text SubLabel;
    [field: SerializeField] private TMP_Text BodyLabel;

    [field: Space(5.0f)]

    [field: SerializeField] private NavigatorButton AcceptInteract;
    [field: SerializeField] private NavigatorButton DenyInteract;

    [field: Space(5.0f)]

    [field: SerializeField] private PromptDataUI DefaultData;

    private PromptDataUI cachedData;

    public void PromptCalled(PromptDataUI data)
    {
        Prompt.SetActive(true);
        SetupPrompt(data);
    }

    private void SetupPrompt(PromptDataUI data)
    {
        TitleLabel.text = data.Title;
        SubLabel.text = data.SubTitle;
        BodyLabel.text = data.Description;

        AcceptInteract.Text.text = data.AcceptButtonText;
        DenyInteract.Text.text = data.DenyButtonText;

        cachedData = data;
    }

    private void NavigatorInteracted(bool DidPlayerAccept)
    {
        Prompt.SetActive(false);
        cachedData.PromptFinalized?.Invoke(DidPlayerAccept);

        SetupPrompt(DefaultData);
    }

    protected override void Initialize()
    {
        Prompt.SetActive(false);
        SetupPrompt(DefaultData);

        AcceptInteract.Button.onClick.AddListener(() => NavigatorInteracted(true));
        DenyInteract.Button.onClick.AddListener(() => NavigatorInteracted(false));
    }
}
