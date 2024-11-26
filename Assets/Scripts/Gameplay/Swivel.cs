using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swivel : MonoBehaviour
{
    // Brooke

    public float xAngle, yAngle, zAngle;
    public float speed;

    public GameObject mopHandle;
    public bool isAttached;

    [Header("Action in Space")]
    public bool worldSpace;
    public bool local;

    private void Awake()
    {
        speed = 10;
        local = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SendIt();
    }


    private void OnCollisionEnter(Collision collision)
    {
        //if ()
        Debug.Log("Player hit mop");
        isAttached = true;
    }

    void SendIt()
    {
        if (isAttached)
        {
            if (worldSpace)
                transform.Rotate(Vector3.up * speed * Time.deltaTime, Space.World);

            //Otherwise, rotate the GameObject in local space
            else
                transform.Rotate(Vector3.up * speed * Time.deltaTime, Space.Self);
        }
    }

    void TimeToJump()
    {

    }
}
