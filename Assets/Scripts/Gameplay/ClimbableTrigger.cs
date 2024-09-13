using UnityEngine;

public class ClimbableTrigger : MonoBehaviour
{
    [field: Header("Override Move Type")]
    [field: Tooltip("Determine whether the player's movement type should be changed through this script. Disable only for debugging.")]
    [field: SerializeField] private bool OverrideMoveType;

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
    private PlayerSystem playSys;

    private void DetermineClimbHook()
    {
        // If Player jumps to pipe, latch on.
        if ((playSys.IsJumpingFromClimb || playSys.IsPlayerJumping()) && !SkipJumpToClimbCheck)
        {
            playSys.IsClimbing = true;
            playSys.IsJumpingFromClimb = false;
            playSys.SetVelocity(Vector3.zero);

            if (!usePipePointYPos) { playSys.Warp(new(PipePoint.transform.position.x, playSys.transform.position.y, PipePoint.transform.position.z)); }
            else { playSys.Warp(PipePoint.transform.position); }

            SkipJumpToClimbCheck = true;
        }

        // Beyond here, they've requested to latch on.
        if (!playSys.ClimbingRequested) return;

        playSys.IsClimbing = !playSys.IsClimbing;
        playSys.SetVelocity(Vector3.zero);

        if (playSys.IsClimbing)
        {
            if (!usePipePointYPos) { playSys.Warp(new(PipePoint.transform.position.x, playSys.transform.position.y, PipePoint.transform.position.z)); }
            else { playSys.Warp(PipePoint.transform.position); }

            //playerSystem.SetMovementType(MovementType.LockToForwardBack);

            SkipJumpToClimbCheck = true;
        }
        else if (!playSys.IsClimbing && useGroundPoint)
        {
            playSys.Warp(GroundPoint.transform.position);

            //playerSystem.SetMovementType(MovementType.LockToLeftRight);

            playSys = null;
            SkipJumpToClimbCheck = false;

            playSys.ClimbingRequested = false;
        }

        playSys.ClimbingRequested = false;
    }


    private void Update()
    {
        if (!playSys) return;

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
        playSys.IsClimbing = false;
    }

    #region OnTrigger Functions
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playSys = other.GetComponent<PlayerSystem>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") || playSys) return;
        playSys = other.GetComponent<PlayerSystem>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (playSys.IsClimbing) { playSys.ClimbingRequested = true; }
        //playerSystem.SetMoveType(MoveType.LockToLeftRight);

        playSys = null;

        SkipJumpToClimbCheck = false;
    }
    #endregion
}