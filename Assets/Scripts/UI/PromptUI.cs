using TMPro;

using UnityEngine;

public class PromptUI : Singleton<PromptUI>
{
    [field: Header("Root")]
    [field: SerializeField] private GameObject Prompt;

    [field: Header("Text Labels")]
    [field: SerializeField] private TMP_Text TitleLabel;
    [field: SerializeField] private TMP_Text SubLabel;
    [field: SerializeField] private TMP_Text BodyLabel;

    [field: Header("Interactions")]
    [field: SerializeField] private NavigatorButton AcceptInteract;
    [field: SerializeField] private NavigatorButton DenyInteract;

    [field: Header("Prompt Data")]
    [field: SerializeField] private PromptDataUI DefaultData;

    private PromptDataUI cachedData;

    public bool PromptActive() => Prompt.activeSelf;

    public void Begin(PromptDataUI data)
    {
        Prompt.SetActive(true);
        SetupPrompt(data);
    }

    public void ForceEnd()
    {
        Prompt.SetActive(false);
        SetupPrompt(DefaultData);
        cachedData = new();
    }

    public void AcceptButtonClicked() => NavigatorInteracted(true);
    public void DenyButtonClicked() => NavigatorInteracted(false);

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
        cachedData = new();

        SetupPrompt(DefaultData);
    }

    protected override void Initialize()
    {
        Prompt.SetActive(false);
        SetupPrompt(DefaultData);
    }
}
