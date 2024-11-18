using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;


/// <summary> This script should not be used anywhere outside Josh's scene. </summary>
public class JoshsSuperUsefulTools : MonoBehaviour
{
    #region Private Variables
    [Header("Bools")]

    [Tooltip("Press the R key to reload the current scene.")]
    [SerializeField] bool enableSceneReloading;
    [Tooltip("Press the Up and Down arrow keys to cycle between referenced sets.")]
    [SerializeField] bool enableSetCycling;

    [Header("Set Cycling")]

    [Tooltip("References to sets that can be toggled. They need to be manually set.\n\nIgnored if Enable Set Cycling is false.")]
    [SerializeField] GameObject[] objectReferences;


    int storedIndex = -1;
    #endregion


    #region Private Functions
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && enableSceneReloading) { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }

        if (enableSetCycling)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                switch (storedIndex)
                {
                    case 0:
                        objectReferences[0].SetActive(false);
                        objectReferences[1].SetActive(true);
                        storedIndex++;
                        break;
                    case 1:
                        objectReferences[1].SetActive(false);
                        objectReferences[2].SetActive(true);
                        storedIndex++;
                        break;
                    case 2:
                        objectReferences[2].SetActive(false);
                        objectReferences[3].SetActive(true);
                        storedIndex++;
                        break;
                    case 3:
                        objectReferences[3].SetActive(false);
                        objectReferences[4].SetActive(true);
                        storedIndex++;
                        break;
                    case 4:
                        objectReferences[4].SetActive(false);
                        objectReferences[5].SetActive(true);
                        storedIndex++;
                        break;
                    case 5:
                        objectReferences[5].SetActive(false);
                        objectReferences[0].SetActive(true);
                        storedIndex = 0;
                        break;
                    default:
                        Debug.LogError("Too many references! Add more references in code!");
                        break;
                }
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                switch (storedIndex)
                {
                    case 0:
                        objectReferences[0].SetActive(false);
                        objectReferences[5].SetActive(true);
                        storedIndex = 5;
                        break;
                    case 1:
                        objectReferences[1].SetActive(false);
                        objectReferences[0].SetActive(true);
                        storedIndex--;
                        break;
                    case 2:
                        objectReferences[2].SetActive(false);
                        objectReferences[1].SetActive(true);
                        storedIndex--;
                        break;
                    case 3:
                        objectReferences[3].SetActive(false);
                        objectReferences[2].SetActive(true);
                        storedIndex--;
                        break;
                    case 4:
                        objectReferences[4].SetActive(false);
                        objectReferences[3].SetActive(true);
                        storedIndex--;
                        break;
                    case 5:
                        objectReferences[5].SetActive(false);
                        objectReferences[4].SetActive(true);
                        storedIndex--;
                        break;
                    default:
                        Debug.Log("Too many references! Add more references in code!");
                        break;
                }
            }
        }
    }


    private void Start()
    {
        // Set all referenced parents to false if they don't equal 0.
        if (enableSetCycling && objectReferences.Length > 0)
        {
            switch (objectReferences.Length)
            {
                case 1:
                    enableSetCycling = false;
                    break;
                default:
                    for (int i = 0; i < objectReferences.Length; i++)
                    {
                        if (objectReferences[i].activeSelf && storedIndex == -1) { storedIndex = i; objectReferences[i].SetActive(true); }
                        else { objectReferences[i].SetActive(false); }
                    }
                    break;
            }

            if (storedIndex == -1) { enableSetCycling = false; }
        }
    }
    #endregion
}
