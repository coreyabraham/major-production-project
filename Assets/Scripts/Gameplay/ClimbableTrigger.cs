using UnityEngine;

public class ClimbableTrigger : MonoBehaviour
{
    public GameObject[] ClimbPoints = new GameObject[2];

    private PlayerSystem playerSystem;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerSystem != null)
        {
            playerSystem.ToggleClimbingMode();
            playerSystem.WarpToPosition(ClimbPoints[1].transform.position);
        }
    }


    #region OnTrigger Functions
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSystem = other.GetComponent<PlayerSystem>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSystem = null;
        }
    }
    #endregion
}
