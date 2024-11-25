using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowAwareness : MonoBehaviour
{
    public HazardTrigger crowSight;
    public bool AwareofPlayer { get; private set; }
    public Vector3 DirectionOfPlayer { get; private set; }

    [SerializeField] float awarenessDistance;
    public Transform player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        MoveToPlayer();
    }

    void MoveToPlayer()
    {
        if(crowSight.hasBeenSpotted == true)
        {
            Vector3 enemyToPlayerVector = player.position;

            AwareofPlayer = (enemyToPlayerVector.magnitude <= awarenessDistance);
        }
    }

}