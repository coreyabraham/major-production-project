using UnityEngine;

public class temp_hide : MonoBehaviour
{
    public GameObject thingToHide;



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            thingToHide.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            thingToHide.SetActive(true);
        }
    }
}
