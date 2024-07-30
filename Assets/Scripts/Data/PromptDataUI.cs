[System.Serializable]
public struct PromptDataUI
{
    public string Title;
    public string SubTitle;
    public string Description;

    public string AcceptButtonText;
    public string DenyButtonText;

    public System.Action<bool> PromptFinalized;
}