using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatLookAtLocator : MonoBehaviour
{
	
	[field: SerializeField] private Transform ratPos;
	[field: SerializeField] private Transform camPos;
	[field: SerializeField] private Vector3 offset;
	

    void Update()
    {
		Vector3 newPos = new Vector3(camPos.position.x + offset.x, 
									 ratPos.position.y + offset.y, 
									 ratPos.position.z + offset.z);
        gameObject.transform.position = newPos;
    }
}