using UnityEngine;

public class DestroyObjectsOnTriggerEnter : MonoBehaviour
{
    private enum DestroyObjectsEnum
    {
        Elements = 0,
        Prefabs
    }

    #region Public Variables
    [field: Header("Main Settings")]

    [field: Tooltip("The types of objects that this trigger will deal with. By default, it should be set to Elements.\n\nSet to \"Prefabs\" if this is being used to destroy prefabs that will be spawned using the \"Falling Object Spawner\" script. Every setting listed under the Elements Type Settings heading will subsequently be ignored.")]
    [field: SerializeField] DestroyObjectsEnum typeOfObject = DestroyObjectsEnum.Elements;
    [field: Tooltip("Should the GameObject that this script is on be destroyed immediately after being triggered?")]
    [field: SerializeField] bool destroySelf;

    [field: Header("Elements Type Settings")]

    [field: Tooltip("The GameObjects that are going to be destroyed when the tagged object enters this trigger.\n\nCan be left empty if \"Type Of Object\" is set to Prefabs.")]
    [field: SerializeField] GameObject[] elements;
    [field: Tooltip("When the scene starts, should all of the elements listed in the array be automatically enabled?\n\nWill be ignored if \"Type Of Object\" is set to Prefabs.")]
    [field: SerializeField] bool enableElementsOnStart;
    [field: Tooltip("The tag of the object that is intended to enter this trigger.\n\nWill be ignored if \"Type Of Object\" is set to Prefabs.")]
    [field: SerializeField] string tagToLookFor;
    #endregion


    private void OnTriggerEnter(Collider other)
    {
        if (tagToLookFor == "" && typeOfObject == DestroyObjectsEnum.Elements) { return; }

        if (other.CompareTag(tagToLookFor))
        {
            foreach (GameObject obj in elements)
            {
                if (!obj) continue;
                Destroy(obj);
            }
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
}
