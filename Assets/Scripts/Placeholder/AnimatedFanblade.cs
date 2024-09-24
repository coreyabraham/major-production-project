using UnityEngine;

/// <summary>
/// Animates the blades within the AC Unit without using actual animations.
/// </summary>
public class AnimatedFanblade : MonoBehaviour
{
    #region Public Variables
    [field: Header("Bool & Object References")]

    [field: Tooltip("Is the fanblade animating?")]
    public bool powered;
    [field: Tooltip("A reference to the blade object.")]
    [field: SerializeField] GameObject blade;
    [field: Tooltip("A reference to the hazard trigger associated with the fanblade.\n\nIf this fanblade doesn't have an associated hazard trigger, leave this section blank. It will function without a reference to one.")]
    [field: SerializeField] GameObject hazardTrigger;

    [field: Header("Audio")]

    [field: Tooltip("Audio for the AC Unit while it's active/powered.")]
    [field: SerializeField] AudioSource unitRunning;
    [field: Tooltip("Audio for the AC Unit when it is deactivated/loses power.")]
    [field: SerializeField] AudioSource unitStopped;
    #endregion

    #region Private Variables
    float currSpeed, maxSpeed = -1000f;
    #endregion

    private void Update()
    {
        if (currSpeed != 0) { blade.transform.Rotate(0, 0, currSpeed * Time.deltaTime); }

        if (powered && currSpeed > maxSpeed) { currSpeed -= 300 * Time.deltaTime; }
        else if (powered && currSpeed < maxSpeed) { currSpeed = maxSpeed; }

        if (!powered && currSpeed < 0) { currSpeed += 195 * Time.deltaTime; }
        else if (!powered && currSpeed > 0) { currSpeed = 0; }

        if (powered && !hazardTrigger.activeSelf) { hazardTrigger.SetActive(true); }
        else if (!powered && hazardTrigger.activeSelf) { hazardTrigger.SetActive(false); }
    }
    private void Start()
    {
        unitStopped.enabled = false;
        unitRunning.enabled = true;
        unitRunning.Play();
    }
}
