using UnityEngine;


// Script that will kill the player if they enter or are detected within the trigger it's attached to.
// The player can avoid being seen if their "IsHidden" bool is enabled.
public class HazardTrigger : MonoBehaviour
{
    float resetTimer = 0;

    public bool useCrowSound, useSnap;

    #region Private Variables
    private PlayerSystem playerSystem;
    private bool hasBeenSpotted;        // Should be reset back to false if the player dies and respawns.
    public AudioSource crowCaw;
    public AudioSource crowSwoop;
    public AudioSource snap;
    #endregion

    #region Functions - Private
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

        playerSystem.DeathTriggered();

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSystem = other.GetComponent<PlayerSystem>();


            if (!playerSystem.IsHidden && !hasBeenSpotted)
            {
                PlayerIsSpotted();
            }
            else
            {
                playerSystem = null;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSystem = other.GetComponent<PlayerSystem>();


            if (!playerSystem.IsHidden && !hasBeenSpotted)
            {
                PlayerIsSpotted();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSystem = null;
        }
    }
    #endregion



    private void Update()
    {
        if (hasBeenSpotted) { resetTimer += Time.deltaTime; }
        if (resetTimer > 1)
        {
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
}
