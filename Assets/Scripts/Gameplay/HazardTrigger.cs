using UnityEngine;


// Script that will kill the player if they enter or are detected within the trigger it's attached to.
// The player can avoid being seen if their "IsHidden" bool is enabled.
public class HazardTrigger : MonoBehaviour, ITouchable
{
    #region Private Variables
    [SerializeField] private bool useCrowSound, useSnap;

    [SerializeField] private AudioSource crowCaw;
    [SerializeField] private AudioSource crowSwoop;
    [SerializeField] private AudioSource snap;

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
        Debug.Log("WWHIUS");
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
