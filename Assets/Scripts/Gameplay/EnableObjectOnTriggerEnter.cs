using System.Collections.Generic;
using UnityEngine;

/// <summary>This script is used to enable GameObjects when a specially tagged object falls through it.</summary>
public class EnableObjectOnTriggerEnter : MonoBehaviour
{
    #region Public Variables
    [field: Tooltip("The GameObjects in the scene that are going to be enabled when the tagged object enters this trigger.")]
    [field: SerializeField] GameObject[] elementsToEnable;

    [field: Tooltip("The tag of the object that is intended to enter this trigger. This does not need to (and should never) be set to \"Player\".")]
    [field: SerializeField] string tagToLookFor;

    [field: Tooltip("Instead of setting the corresponding GameObject as active, disregard that information and just enable a random object from the array. A check is put in place to prevent it from enabling an object that's already enabled.")]
    [field: SerializeField] bool randomiseEnables;
    [field: Tooltip("When the tagged object enters this trigger, should it be destroyed?")]
    [field: SerializeField] bool destroyTaggedObject;
    [field: Tooltip("If the player enters this trigger, will it be treated as a death?")]
    [field: SerializeField] bool canKillPlayer;
    #endregion

    private void UpdateVisibilityRandomly()
    {
        List<GameObject> invisibleObjs = new();

        foreach (GameObject obj in elementsToEnable)
        {
            if (obj.activeInHierarchy) continue;
            invisibleObjs.Add(obj);
        }

        int result = Random.Range(0, invisibleObjs.Count - 1);
        invisibleObjs[result].SetActive(true);
    }

    private void UpdateVisibilitySpecifically(string nameOfObject)
    {
        switch (nameOfObject)
        {
            case "FallingPotPlant": elementsToEnable[2].SetActive(true); break;
            case "FallingSprayCan": elementsToEnable[1].SetActive(true); break;
            case "FallingPipe": elementsToEnable[0].SetActive(true); break;
            case "FallingNoodleBox": elementsToEnable[0].SetActive(true); break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (tagToLookFor == "") { return; }

        if (other.CompareTag(tagToLookFor))
        {
            if (randomiseEnables) { UpdateVisibilityRandomly(); }
            else { UpdateVisibilitySpecifically(other.name); }

            if (destroyTaggedObject) { Destroy(other.gameObject); }
        }

        if (other.CompareTag("Player") && canKillPlayer)
        {
            PlayerSystem playSys = other.GetComponent<PlayerSystem>();
            playSys.DeathTriggered();
        }
    }

    private void Start()
    {
        foreach (GameObject obj in elementsToEnable) obj.SetActive(true);
    }
}
