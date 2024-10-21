using UnityEngine;


// Script that will kill the player if they enter or are detected within the trigger it's attached to.
// The player can avoid being seen if their "IsHidden" bool is enabled.
public class HazardTrigger : MonoBehaviour, ITouchable
{
    #region Public Variables
    [field: Header("ITouchable Inherited")]
    [field: SerializeField] public bool Enabled { get; set; } = true;
    [field: SerializeField] public bool HideOnStartup { get; set; } = false;

    [field: Header("Audio")]
    public bool useCrowSound;
    public bool useSnap;

    public AudioSource crowCaw;
    public AudioSource crowSwoop;
    public AudioSource snap;
    #endregion

    #region Private Variables
    private float resetTimer = 0;
    private bool hasBeenSpotted;        // Should be reset back to false if the player dies and respawns.
    #endregion

    #region Functions - Public
    public void ResetSpottedStatus() => hasBeenSpotted = false;
    #endregion

    #region Functions - Private
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

        Player.DeathTriggered();
    }

    public void Entered(PlayerSystem Player)
    {
        if (Player.IsHidden) return;
        PlayerIsSpotted(Player);
    }

    public void Left(PlayerSystem Player) {  }

    public void Staying(PlayerSystem Player)
    {
        if (Player.IsHidden) return;
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
