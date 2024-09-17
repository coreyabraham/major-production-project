using UnityEngine;

public class TEMP_chewThroughWire : MonoBehaviour
{
    [field: SerializeField] GameObject destroyThis;
    [field: SerializeField] TEMP_ACfanblade fanblade;
    public AudioSource FanOn;
    public AudioSource FanOff;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Destroy(destroyThis);
                fanblade.poweredOn = false;
                FanOn.enabled = false;
                FanOff.enabled = true;
                Destroy(gameObject);
            }
        }
    }
}
