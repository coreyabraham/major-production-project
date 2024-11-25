using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowMovement : MonoBehaviour
{
    // Movement
    private Vector3 targetDirection;

    // Speed
    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;

    // Collect components
    private Rigidbody crowRb;
    private CrowAwareness crowAwareness;


    void Awake()
    {
        crowRb = GetComponent<Rigidbody>();
        crowAwareness = GetComponent<CrowAwareness>();
    }

    // Call these functions
    void FixedUpdate()
    {
        PlayerTargeting();
    }

    void PlayerTargeting()
    {
        if (crowAwareness.AwareofPlayer)
        {
            RotateToTarget();
            targetDirection = crowAwareness.DirectionOfPlayer;
            crowRb.velocity = transform.forward * speed;
        }
        else return;
    }

    /* Ignore the enemy rotation - treat that as an emergent graphical feature.
     * Constantly move the enemy forward.
     * Add its target velocity to its current velocity, then renormalise.*/
    void RotateToTarget()
    {
        float desiredAngle = Mathf.Atan2(targetDirection.x, targetDirection.z);
        transform.localEulerAngles = new Vector3(0, desiredAngle, 0) * rotationSpeed;

        Debug.Log("Using angle: " + desiredAngle);
    }
}