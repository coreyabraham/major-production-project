using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatLookAtLocator : MonoBehaviour
{
	
	[field: SerializeField] private Transform ratPos;
	[field: SerializeField] private Transform camPos;

    void Update()
    {
		Vector3 newPos = new Vector3(camPos.position.x, ratPos.position.y, ratPos.position.z);
        gameObject.transform.position = newPos;
    }
}