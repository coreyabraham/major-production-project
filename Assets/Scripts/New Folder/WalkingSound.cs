using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingSound : MonoBehaviour
{

    public AudioSource walk;

    // Update is called once per frame
    void PlaySteps()
    {      
            walk.Play();
            Debug.Log("PLAY");
              
    }
}
