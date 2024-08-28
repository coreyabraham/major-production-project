using UnityEngine;

public class ClimbableTrigger : MonoBehaviour
{
    [field: Header("Pipe Point Variables")]
    [field: Tooltip("The point in world space that the player will warp to when attaching to a pipe.")]
    [field: SerializeField] private GameObject PipePoint;
    [field: Tooltip("Determines if the y-position of \"Pipe Point\" should be used when warping the player to it. If false, the player will warp to this point and use its current y-position instead.")]
    [field: SerializeField] private bool usePipePointYPos;

    [field: Header("Ground Point Variables")]
    [field: Tooltip("The point in world space that the player will warp to when detaching from the pipe. Can be left empty if \"Use Ground Point\" is false.")]
    [field: SerializeField] private GameObject GroundPoint;
    [field: Tooltip("Determines if the \"Ground Point\" variable should be used. If set as false, then \"Ground Point\" can be left empty.")]
    [field: SerializeField] private bool useGroundPoint;

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

            if (playerSystem.IsClimbing)
            {
                if (!usePipePointYPos) { playerSystem.Warp(new(PipePoint.transform.position.x, playerSystem.transform.position.y, PipePoint.transform.position.z)); }
                else { playerSystem.Warp(PipePoint.transform.position); }

                SkipJumpToClimbCheck = true;
            }
            else if (!playerSystem.IsClimbing && useGroundPoint)
            {
                playerSystem.Warp(GroundPoint.transform.position);
            SkipJumpToClimbCheck = true;
        }
        else if (!playerSystem.IsClimbing && useGroundPoint)
        {
            playerSystem.Warp(GroundPoint.transform.position);

                playerSystem = null;
                SkipJumpToClimbCheck = false;
            }

            playerSystem.ClimbingRequested = false;
        }

        playerSystem.ClimbingRequested = false;
    }

    private void Update()
    {
        if (!playerSystem) return;

        DetermineClimbHook();

        bool isOutsideBounds = false;

        #region Buggy Pipe Detach
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
        #endregion

        if (!isOutsideBounds) return;
        playerSystem.IsClimbing = false;
    }

    #region OnTrigger Functions
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerSystem = other.GetComponent<PlayerSystem>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") || playerSystem == null) return;
        playerSystem = other.GetComponent<PlayerSystem>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (playerSystem.IsClimbing) { playerSystem.ClimbingRequested = true; }
        playerSystem = null;

        SkipJumpToClimbCheck = false;
    }
    #endregion
}