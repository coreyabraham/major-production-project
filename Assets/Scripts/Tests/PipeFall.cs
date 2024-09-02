using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeFall : MonoBehaviour
{
    public bool gravityEnabled = false;
    new public Rigidbody pipeRb;
    private readonly static float initialTime = 1f;
    private float timer = 0;

    void Update()
    {
     
    }

    // Start is called before the first frame update
    void Awake()
    {
        pipeRb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            gravityEnabled = true;
            if (gravityEnabled)
            {
                timer += Time.deltaTime;
                if (timer >= initialTime)
                {
                    pipeRb.useGravity = true;
                }
            }
            else
            {
                timer = 0; // reset timer
            }
        }
        else return;
    }
}
