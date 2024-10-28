using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class ActivateFalling : MonoBehaviour
{
    [SerializeField] FallingObjectSpawner[] spawnerScript;
    [SerializeField] Collider[] spawnColliders;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("It's the player!");
            for (int i = 0; i<spawnerScript.Length; i++)
            {
                spawnerScript[i].isActive = true;
            }
        }
    }
}
