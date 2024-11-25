using UnityEngine;


// This script is now used simply for additional Death Stuff related to the player.
// As of now, it handles sounds.
public class HazardTrigger : MonoBehaviour, ITouchable
{
    #region Private Variables
    [field: Header("Bools")]
    [field: SerializeField] private bool useCrowSound;
    [field: SerializeField] private bool useSnap;

    [field: Header("Audio Sources")]
    [field: SerializeField] private AudioSource crowCaw;
    [field: SerializeField] private AudioSource crowSwoop;
    [field: SerializeField] private AudioSource snap;

    private float resetTimer = 0;
    public bool hasBeenSpotted;        // Should be reset back to false if the player dies and respawns.

    public bool Enabled { get; set; } = true;
    public bool HideOnStartup { get; set; } = false;
    #endregion

    #region Functions - Private
    private void PlayerIsSpotted()
    {
        if (useCrowSound)
        {
            crowCaw.enabled = true;
            crowSwoop.enabled = true;
        }

        if (useSnap)
        {
            snap.enabled = true;
        }
    }

    public void Entered(PlayerSystem Player)
    {
        if (!Player) { return; }
        hasBeenSpotted = true;
    }

    public void Left(PlayerSystem Player) { }

    public void Staying(PlayerSystem Player) { }
    #endregion

    private void Update()
    {
        if (hasBeenSpotted) { resetTimer += Time.deltaTime; }
        if (resetTimer < 1) { return; }

        PlayerIsSpotted();
        hasBeenSpotted = false;
        resetTimer = 0;
    }
}
