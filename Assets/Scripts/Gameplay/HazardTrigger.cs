using UnityEngine;


// Script that will kill the player if they enter or are detected within the trigger it's attached to.
// The player can avoid being seen if their "IsHidden" bool is enabled.
public class HazardTrigger : MonoBehaviour
{
    #region Private Variables
    private PlayerSystem playerSystem;
    private bool hasBeenSpotted;        // Should be reset back to false if the player dies and respawns.
    #endregion

    #region Functions - Private
    private void PlayerIsSpotted()
    {
        hasBeenSpotted = true;
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
}
