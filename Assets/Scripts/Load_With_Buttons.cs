using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Load_With_Buttons : MonoBehaviour
{
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            SceneManager.LoadScene(1);
        }

        if (Input.GetKeyDown("2"))
        {
            SceneManager.LoadScene(2);
        }

        if (Input.GetKeyDown("3"))
        {
            SceneManager.LoadScene(3);
        }

        if (Input.GetKeyDown("4"))
        {
            SceneManager.LoadScene(4);
        }
    }
}
