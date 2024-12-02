using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RatAlign : MonoBehaviour
{
    public Transform targetPos;
    public Transform returnPos;
    public GameObject ratbody;
    public GameObject playerV3;
    [SerializeField] float targetSpeed;
    [SerializeField] float returnSpeed;
    [SerializeField] float threshold = 0.1f;
    public bool isInside;


    private void FixedUpdate()
    {
        if (isInside)
        {
                Vector3 newPos = new Vector3(playerV3.transform.position.x, playerV3.transform.position.y, targetPos.position.z);
                Vector3 lerpPos = Vector3.Lerp(ratbody.transform.position, newPos, targetSpeed * Time.deltaTime);
                ratbody.transform.position = lerpPos;
                Debug.Log(ratbody.name + " is moving to target.");
        }
        else
        {
                Vector3 newPos = new Vector3(playerV3.transform.position.x, playerV3.transform.position.y, returnPos.position.z);
                Vector3 lerpPos = Vector3.Lerp(ratbody.transform.position, newPos, targetSpeed * Time.deltaTime);
                ratbody.transform.position = lerpPos;
                Debug.Log(ratbody.name + " is moving to target.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ratbody)
        {
            isInside = true;          
        }
    }

    private void OnTriggerExit(Collider other)
    {
            isInside = false;
    }
}
