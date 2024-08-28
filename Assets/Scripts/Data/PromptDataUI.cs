/// <summary>
/// A class for the `Title Screen` Scene that dictates what data the `PromptUI` class uses.
/// </summary>
[System.Serializable]
public struct PromptDataUI
{
    /// <summary>
    /// The string text the Title TMP_Text Component uses.
    /// </summary>
    public string Title;

    /// <summary>
    /// The string text the Subtitle TMP_Text Component uses.
    /// </summary>
    public string SubTitle;

    /// <summary>
    /// The string text the Description TMP_Text Component uses.
    /// </summary>
    public string Description;

    /// <summary>
    /// The string text the Accept Button Component uses.
    /// </summary>
    public string AcceptButtonText;

    /// <summary>
    /// The string text the Deny Button Component uses.
    /// </summary>
    public string DenyButtonText;

    /// <summary>
    /// The method that gets called once either the Accept or Deny Buttons are invoked.
    /// </summary>
    public System.Action<bool> PromptFinalized;
}