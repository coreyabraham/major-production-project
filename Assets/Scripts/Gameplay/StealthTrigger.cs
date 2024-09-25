using UnityEngine;

/// <summary>
/// Trigger used for parts of stealth sections that are deemed to be "safe zones".
/// These triggers can exist within the hazard triggers and still function.
/// </summary>
public class StealthTrigger : MonoBehaviour
{
    #region Private Variables
    private PlayerSystem playerSystem;
    #endregion

    #region Functions - Private
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) { return; }
        playerSystem.IsHidden = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || !playerSystem) { return; }

        playerSystem.IsHidden = false;
        playerSystem = null;
    }
    #endregion
}
