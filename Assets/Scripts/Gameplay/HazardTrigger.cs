using UnityEngine;


// Script that will kill the player if they enter or are detected within the trigger it's attached to.
// The player can avoid being seen if their "IsHidden" bool is enabled.
public class HazardTrigger : MonoBehaviour, ITouchable
{
    float resetTimer = 0;

    public bool useCrowSound, useSnap;

    #region Private Variables
    private bool hasBeenSpotted;        // Should be reset back to false if the player dies and respawns.
    public AudioSource crowCaw;
    public AudioSource crowSwoop;
    public AudioSource snap;

    public bool Enabled { get; set; } = true;
    public bool HideOnStartup { get; set; } = false;
    #endregion

    #region Functions - Private
    private bool HasPlayerBeenSpotted() => !GameSystem.Instance.Player.IsHidden && !hasBeenSpotted;

    private void PlayerIsSpotted()
    {
        hasBeenSpotted = true;

        if (useCrowSound)
        {
            crowCaw.enabled = true;
            crowSwoop.enabled = true;
        }

        if (useSnap)
        {
            snap.enabled = true;
        }

        GameSystem.Instance.Player.DeathTriggered();
    }

    public void Entered(Collider other)
    {
        if (!HasPlayerBeenSpotted())
        PlayerIsSpotted();
    }

    public void Left(Collider other) {  }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!HasPlayerBeenSpotted()) return;
        PlayerIsSpotted();
    }
    #endregion

    private void Update()
    {
        if (hasBeenSpotted) { resetTimer += Time.deltaTime; }
        if (resetTimer < 1) { return; }

        if (useCrowSound)
        {
            crowCaw.enabled = false;
            crowSwoop.enabled = false;
        }

        if (useSnap) { snap.enabled = false; }

        hasBeenSpotted = false;
        resetTimer = 0;
    }
}
