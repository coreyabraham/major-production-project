using UnityEngine;

public class BoxResetPosition : MonoBehaviour
{
    [SerializeField] bool hideOnStart = true;


    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Grabbable")) { return; }

        BoxScript bs = other.GetComponent<BoxScript>();
        bs.gameObject.transform.position = bs.GetStartPos();
    }


    private void Start()
    {
        if (hideOnStart) { GetComponent<MeshRenderer>().enabled = false; }
    }
}
