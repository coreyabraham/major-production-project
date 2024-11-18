using UnityEngine;


// Script that will kill the player if they enter or are detected within the trigger it's attached to.
// The player can avoid being seen if their "IsHidden" bool is enabled.
public class HazardTrigger : MonoBehaviour, ITouchable
{
    #region Private Variables
    [field: Header("Timer")]
    [field: Tooltip("The amount of time in seconds before the player is considered \"dead\".")]
    [field: SerializeField] private float deathDelayTimer;

    [field: Header("Bools")]
    [field: SerializeField] private bool useCrowSound;
    [field: SerializeField] private bool useSnap;

    [field: Header("Audio Sources")]
    [field: SerializeField] private AudioSource crowCaw;
    [field: SerializeField] private AudioSource crowSwoop;
    [field: SerializeField] private AudioSource snap;

    private float resetTimer = 0;
    private bool hasBeenSpotted;        // Should be reset back to false if the player dies and respawns.

    public bool Enabled { get; set; } = true;
    public bool HideOnStartup { get; set; } = false;
    #endregion

    #region Functions - Private
    private bool HasPlayerBeenSpotted(PlayerSystem Player) => Player.IsHidden && !hasBeenSpotted;

    private void PlayerIsSpotted(PlayerSystem Player)
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

        // Put timer delay

        Player.DeathTriggered();
    }

    public void Entered(PlayerSystem Player)
    {
        if (Player.IsHidden) return;
        if (!HasPlayerBeenSpotted(Player))
        PlayerIsSpotted(Player);
    }

    public void Left(PlayerSystem Player) {  }

    public void Staying(PlayerSystem Player)
    {
        if (Player.IsHidden) return;
        if (!HasPlayerBeenSpotted(Player)) return;
        PlayerIsSpotted(Player);
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
