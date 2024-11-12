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
			if(swapOut != null) { swapOut.enabled = false; }
			if(swapIn != null) { swapIn.enabled = true; }
		}
	}
}
