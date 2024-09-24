using UnityEngine;

public class TEMP_chewThroughWire : MonoBehaviour
{
    [field: SerializeField] GameObject destroyThis;
    [field: SerializeField] AnimatedFanblade fanblade;
    public AudioSource FanOn;
    public AudioSource FanOff;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Destroy(destroyThis);
                fanblade.powered = false;
                FanOn.enabled = false;
                FanOff.enabled = true;
                Destroy(gameObject);
            }
        }
    }
}
