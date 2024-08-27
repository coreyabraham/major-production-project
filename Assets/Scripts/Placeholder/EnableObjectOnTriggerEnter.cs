using UnityEngine;

/// <summary>This script is used to enable GameObjects when a specially tagged object falls through it.</summary>
public class EnableObjectOnTriggerEnter : MonoBehaviour
{
    #region Public Variables
    [field: Tooltip("The GameObjects in the scene that are going to be enabled when the tagged object enters this trigger.")]
    [field: SerializeField] GameObject[] elementsToEnable;

    [field: Tooltip("The tag of the object that will enter this trigger.")]
    [field: SerializeField] string tagToLookFor;

    [field: Tooltip("When the tagged object enters this trigger, decide whether or not to destroy it.")]
    [field: SerializeField] bool destroyTaggedObject;
    [field: Tooltip("If the player enters this trigger, will it be treated as a death?")]
    [field: SerializeField] bool canKillPlayer;
    #endregion


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagToLookFor))
        {
            for (int i = 0; i < elementsToEnable.Length; i++) { elementsToEnable[i].SetActive(true); }

            if (destroyTaggedObject) { Destroy(other); }
        }

        if (other.CompareTag("Player") && canKillPlayer)
        {
            PlayerSystem playSys = other.GetComponent<PlayerSystem>();
            playSys.DeathTriggered();
        }
    }


    private void Start()
    {
        for (int i = 0; i < elementsToEnable.Length; i++) { elementsToEnable[i].SetActive(false); }
    }
}
