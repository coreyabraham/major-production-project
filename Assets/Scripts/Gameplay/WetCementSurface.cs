using UnityEngine;


public class WetCementSurface : MonoBehaviour
{
    private PlayerSystem playerSystem;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSystem = other.GetComponent<PlayerSystem>();
            playerSystem.IsOnWetCement = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSystem.IsOnWetCement = false;
            playerSystem = null;
        }
    }
}
