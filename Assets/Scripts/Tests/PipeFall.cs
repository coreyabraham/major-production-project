using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeFall : MonoBehaviour
{
    public Rigidbody pipeRb;
    private readonly static float initialTime = 1f;
    private float timer = 0;

    void Update()
    {
     
    }

    // Start is called before the first frame update
    void Awake()
    {
        pipeRb.constraints = RigidbodyConstraints.FreezePositionY;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pipeRb.constraints = RigidbodyConstraints.None;
            Debug.Log("HELLO?");
            timer += Time.deltaTime;
            if (timer >= initialTime)
            {
                Destroy(this);
            }
            else
            {
                timer = 0; // reset timer
            }
        }
    }
}
