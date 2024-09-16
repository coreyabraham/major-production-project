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

        if (other.CompareTag(tagToLookFor))
        {
            if (elements.Length > 1)
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    if (elements[i]) { Destroy(elements[i]); }
                }
            }
            else
            {
                Destroy(elements[0]);
            }
            

            if (destroySelf) { Destroy(this); }
        }
    }


    private void Start()
    {
        if (enableElementsOnStart)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                elements[i].SetActive(true);
            }
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
