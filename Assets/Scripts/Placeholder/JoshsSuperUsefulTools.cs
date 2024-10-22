using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary> This script should not be used anywhere outside Josh's scene. </summary>
public class JoshsSuperUsefulTools : MonoBehaviour
{
    [Header("Bools n' Shit")]

    [Tooltip("Press the R key to reload the scene.")]
    [SerializeField] bool SceneReloading;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && SceneReloading) { SceneManager.LoadScene("Josh-Testing"); }
    }
}
