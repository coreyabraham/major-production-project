using UnityEngine;

/// <summary>
/// Goes on a trigger box and checks for the Player. It then removes the box from the player and classes them as unhidden.
/// </summary>
public class RemoveBoxFromPlayer : MonoBehaviour
{
    bool hasTriggeredOnce = false;


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || hasTriggeredOnce) { return; }
        
        PlayerSystem playSys = other.GetComponent<PlayerSystem>();
        playSys.IsHidden = false;

        Destroy(playSys.gameObject.transform.GetChild(1).gameObject);

        hasTriggeredOnce = true;
    }
}
