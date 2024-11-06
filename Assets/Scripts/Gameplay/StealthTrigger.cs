using UnityEngine;

/// <summary>
/// Trigger used for parts of stealth sections that are deemed to be "safe zones".
/// These triggers can exist within the hazard triggers and still function.
/// </summary>
public class StealthTrigger : MonoBehaviour, ITouchable
{
    #region Private Variables
    [field: Header("ITouchable Inheritance")]
    public bool Enabled { get; set; } = true;
    public bool HideOnStartup { get; set; } = false;
    #endregion

    #region Functions - Public
    public void Entered(PlayerSystem Player) => Player.IsHidden = true;
    public void Left(PlayerSystem Player) => Player.IsHidden = false;
    public void Staying(PlayerSystem Player) { }
    #endregion
}
