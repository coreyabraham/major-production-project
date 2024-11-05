using UnityEngine;

public class HideOnStartup : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
}