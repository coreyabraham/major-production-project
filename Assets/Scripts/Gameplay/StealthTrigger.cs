using UnityEngine;


// If the player enters this trigger and is not classed as "hidden", trigger death.
public class StealthTrigger : MonoBehaviour
{
    PlayerSystem playerSystem;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSystem = other.GetComponent<PlayerSystem>();

          /* if (!playerSystem.isHidden)
             {
                 // Kill player.
             }
             else
             {
                 // playerSystem = null;
             }
           */


            // can't do this because I need to edit the player script.
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSystem = null;
        }
    }
}
