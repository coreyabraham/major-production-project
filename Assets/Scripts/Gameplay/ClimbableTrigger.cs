using UnityEngine;

public class ClimbableTrigger : MonoBehaviour
{
    [field: SerializeField] private GameObject PipePoint;

    private bool SkipJumpToClimbCheck = false;  // Used to disallow player to reattach to pipe they're already in the trigger of.
    private PlayerSystem playerSystem;

    private void DetermineClimbHook()
    {
        if (playerSystem.IsJumpingFromClimb && !SkipJumpToClimbCheck)
        {
            playerSystem.IsClimbing = true;
            playerSystem.IsJumpingFromClimb = false;
            playerSystem.SetVelocity(Vector3.zero);

            SkipJumpToClimbCheck = true;
        }

        if (!playerSystem.ClimbingRequested) return;

        playerSystem.IsClimbing = !playerSystem.IsClimbing;
        playerSystem.SetVelocity(Vector3.zero);

        if (playerSystem.IsClimbing)
        {
            playerSystem.Warp(PipePoint.transform.position);
            SkipJumpToClimbCheck = true;
        }
        
        playerSystem.ClimbingRequested = false;
    }

    private void Update()
    {
        if (!playerSystem) return;

        DetermineClimbHook();

        bool isOutsideBounds = false;

        /*Buggy
         * - Auto - Detach from Pipe if you pass beyond it's PipePoint transform.
        //if (playerSystem.IsClimbing && toggleCooldownTimer <= 0)
        //{
        //    if (PipePointType == ClimbPointType.PipeNorth &&        // Exit via Northern Pipe
        //        playerSystem.transform.position.y > PipePoint.transform.position.y)
        //    {
        //        isOutsideBounds = true;
        //    }
        //    else if (PipePointType == ClimbPointType.PipeEast &&    // Exit via Eastern Pipe
        //        playerSystem.transform.position.x > PipePoint.transform.position.x)
        //    {
        //        isOutsideBounds = true;
        //    }
        //    else if (PipePointType == ClimbPointType.PipeSouth &&   // Exit via Southern Pipe
        //        playerSystem.transform.position.y < PipePoint.transform.position.y)
        //    {
        //        isOutsideBounds = true;
        //    }
        //    else if (PipePointType == ClimbPointType.PipeWest &&    // Exit via Western Pipe
        //        playerSystem.transform.position.x < PipePoint.transform.position.x)
        //    {
        //        isOutsideBounds = true;
        //    }
        //}
        */

        if (!isOutsideBounds) return;
        playerSystem.IsClimbing = false;
    }


    #region OnTrigger Functions
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerSystem = other.GetComponent<PlayerSystem>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerSystem = null;

        SkipJumpToClimbCheck = false;
    }
    #endregion
}