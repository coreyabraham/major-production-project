using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveTrigger : MonoBehaviour
{
    // Brooke wrote this

    public GameObject triggerToKill;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        { Destroy(triggerToKill); }
        Debug.Log("Destroyed " + triggerToKill.name);
    }
}
