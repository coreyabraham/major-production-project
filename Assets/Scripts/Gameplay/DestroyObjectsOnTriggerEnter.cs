using UnityEngine;

public class DestroyObjectsOnTriggerEnter : MonoBehaviour
{
    #region Public Variables
    [field: Tooltip("The GameObjects that are going to be destroyed when the tagged object enters this trigger.")]
    [field: SerializeField] GameObject[] elements;

    [field: Tooltip("The tag of the object that is intended to enter this trigger.")]
    [field: SerializeField] string tagToLookFor;

    [field: Tooltip("When the scene starts, should all of the elements listed in the array be automatically enabled?")]
    [field: SerializeField] bool enableElementsOnStart;
    [field: Tooltip("Should the GameObject that this script is on be destroyed immediately after the elements in the array are?")]
    [field: SerializeField] bool destroySelf;
    #endregion
    private void OnTriggerEnter(Collider other)
    {
        if (tagToLookFor == "") { return; }
        if (!other.CompareTag(tagToLookFor)) { return; }

        foreach (GameObject obj in elements)
        {
            if (!obj) continue;
            Destroy(obj);
        }

        if (!destroySelf) { return; }

        Destroy(this);
    }


    private void Start()
    {
        if (!enableElementsOnStart) return;

        foreach (GameObject obj in elements)
        {
            obj.SetActive(true);
        }
    }

    /*
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
    }*/
}
