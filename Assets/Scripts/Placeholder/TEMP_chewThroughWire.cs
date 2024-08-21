using UnityEngine;

public class TEMP_chewThroughWire : MonoBehaviour
{
    [field: SerializeField] GameObject destroyThis;
    [field: SerializeField] TEMP_ACfanblade fanblade;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Destroy(destroyThis);
                fanblade.poweredOn = false;
                Destroy(gameObject);
            }
        }
    }
}
