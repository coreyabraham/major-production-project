using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Animates the blades within the AC Unit without using actual animations. This needs to be placed on the trigger for chewing through the wire.
/// </summary>
public class FanbladeHazard : MonoBehaviour, IInteractable
{
    #region Public Variables
    [field: Header("References")]

    [field: Tooltip("A reference to the blade object.")]
    [field: SerializeField] GameObject blade;
    [field: Tooltip("A reference to the hazard trigger associated with the fanblade.\n\nIf this fanblade doesn't have an associated hazard trigger, leave this section blank. It will function without a reference to one.")]
    [field: SerializeField] GameObject hazardTrigger;
    [field: Tooltip("A reference to the smoke particle system.")]
    [field: SerializeField] VisualEffect smokeParticle;

    [field: Header("Bools")]

    [field: Tooltip("Is the fanblade in the AC Unit animating?\n\nNote that toggling this bool will gradually slow down like a real fanblade.")]
    public bool unitIsPowered;

    [field: Header("Audio")]

    [field: Tooltip("Audio for the AC Unit while it's active/powered.")]
    [field: SerializeField] AudioSource unitRunning;
    [field: Tooltip("Audio for the AC Unit when it is deactivated/loses power.")]
    [field: SerializeField] AudioSource unitStopped;

    [field: Header("IInteractable Inheritance")]
    [field: SerializeField] public float InteractionRange { get; set; } = 1.0f;
    [field: SerializeField] public bool IgnoreInteractionRange { get; set; } = false;
    #endregion

    #region Private Variables
    float currSpeed, maxSpeed = -1000f;
    bool hasBeenDeactivated = false;
    #endregion

    public void Interacted(PlayerSystem Player)
    {
        if (hasBeenDeactivated) { return; }

        hazardTrigger.SetActive(false);
        unitIsPowered = false;
        unitRunning.enabled = false;
        unitStopped.enabled = true;
        smokeParticle.SetFloat("SteamAmount", 0);
    }

    private void Update()
    {
        if (currSpeed != 0) { blade.transform.Rotate(0, 0, currSpeed * Time.deltaTime); }

        if (unitIsPowered && currSpeed > maxSpeed) { currSpeed -= 300 * Time.deltaTime; }
        else if (unitIsPowered && currSpeed < maxSpeed) { currSpeed = maxSpeed; }

        if (!unitIsPowered && currSpeed < 0) { currSpeed += 195 * Time.deltaTime; }
        else if (!unitIsPowered && currSpeed > 0) { currSpeed = 0; }

        if (unitIsPowered && !hazardTrigger.activeSelf) { hazardTrigger.SetActive(true); }
        else if (!unitIsPowered && hazardTrigger.activeSelf) { hazardTrigger.SetActive(false); }
    }

    private void Start()
    {
        unitStopped.enabled = false;
        unitRunning.enabled = true;
        unitRunning.Play();
    }
}
