using UnityEngine;

public class BoxReset : MonoBehaviour
{

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Grabbable")) { return; }


    }
}
