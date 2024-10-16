using UnityEngine;

/// <summary>
/// Trigger used for parts of stealth sections that are deemed to be "safe zones".
/// These triggers can exist within the hazard triggers and still function.
/// </summary>
public class StealthTrigger : MonoBehaviour, ITouchable
{
    #region Private Variables
    private PlayerSystem playerSystem;

    [field: Header("ITouchable Inheritance")]
    public bool Enabled { get; set; } = true;
    public bool HideOnStartup { get; set; } = false;
    #endregion

    #region Functions - Public
    public void Entered(PlayerSystem Player) => playerSystem.IsHidden = true;
    public void Left(PlayerSystem Player) => playerSystem.IsHidden = false;
    public void Staying(PlayerSystem Player) { }
    #endregion

    #region Functions - Private
    private void ExternalsCached(PlayerSystem Player, CameraSystem Camera)
    {
        playerSystem = Player;
        GameSystem.Instance.Events.ExternalsCached.RemoveListener(ExternalsCached);
    }

    private void Start() => GameSystem.Instance.Events.ExternalsCached.AddListener(ExternalsCached);
    #endregion
}
