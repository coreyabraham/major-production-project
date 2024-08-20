using UnityEngine;

public class TEMP_enable : MonoBehaviour
{
    public GameObject ThingToEnable, OtherThingToEnable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("special"))
        {
            ThingToEnable.SetActive(true);
            OtherThingToEnable.SetActive(true);
            Destroy(other);
        }
    }


    private void Start()
    {
        ThingToEnable.SetActive(false);
        OtherThingToEnable.SetActive(false);
    }
}
