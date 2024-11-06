using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoneSwapOnTriggerEnter : MonoBehaviour
{
	
	[field: SerializeField] private BoxCollider swapOut;
	[field: SerializeField] private BoxCollider swapIn;
    
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			swapOut.enabled = false;
			swapIn.enabled = true;
		}
	}
}
