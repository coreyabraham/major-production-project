using UnityEngine;

public class PlrDeathState : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        other.GetComponent<PlayerSystem>().DeathTriggered();
    }
}
