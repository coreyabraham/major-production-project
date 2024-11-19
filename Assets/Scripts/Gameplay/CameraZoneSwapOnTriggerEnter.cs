using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoneSwapOnTriggerEnter : MonoBehaviour
{
	
	[field: SerializeField] private BoxCollider[] swapOut;
	[field: SerializeField] private BoxCollider[] swapIn;
    
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			if(swapOut.Length != 0) 
			{ 
				for(int i = 0; i < swapOut.Length; i++)
				{
					swapOut[i].enabled = false;
				}
			}
			if(swapIn.Length != 0) 
			{ 
				for(int i = 0; i < swapIn.Length; i++)
				{
					swapIn[i].enabled = true;
				}
			}
		}
	}
}
