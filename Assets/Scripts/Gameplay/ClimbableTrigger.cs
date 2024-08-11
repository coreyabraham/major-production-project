using Unity.VisualScripting;
using UnityEngine;

public class ClimbableTrigger : MonoBehaviour
{
    public GameObject PipePoint;
    public GameObject GroundPoint;

    private PlayerSystem playerSystem;
    private ClimbPointType PipePointType;


    private void Start()
    {
        PipePointType = PipePoint.GetComponent<ClimbPoint>().climbPointType;
    }

    private void Update()
    {
        if (playerSystem != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                playerSystem.ToggleClimbingMode();

                if (playerSystem.isClimbing)
                {
                    playerSystem.WarpToPosition(PipePoint.transform.position);
                }
                else
                {
                    playerSystem.WarpToPosition(GroundPoint.transform.position);
                }
            }


            /* Buggy - Auto-Detach from Pipe if you pass beyond it's PipePoint transform.
             * 
             * 
             * if (playerSystem.isClimbing && toggleCooldownTimer <= 0)
            {
                if (PipePointType == ClimbPointType.PipeNorth &&        // Exit via Northern Pipe
                    playerSystem.transform.position.y > PipePoint.transform.position.y)
                {
                    TogglePlayerState();
                }
                else if (PipePointType == ClimbPointType.PipeEast &&    // Exit via Eastern Pipe
                    playerSystem.transform.position.x > PipePoint.transform.position.x)
                {
                    TogglePlayerState();
                }
                else if (PipePointType == ClimbPointType.PipeSouth &&   // Exit via Southern Pipe
                    playerSystem.transform.position.y < PipePoint.transform.position.y)
                {
                    TogglePlayerState();
                }
                else if (PipePointType == ClimbPointType.PipeWest &&    // Exit via Western Pipe
                    playerSystem.transform.position.x < PipePoint.transform.position.x)
                {
                    TogglePlayerState();
                }
            }*/
        }
    }


    private void TogglePlayerState()
    {
        playerSystem.ToggleClimbingMode();
        playerSystem.WarpToPosition(GroundPoint.transform.position);
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