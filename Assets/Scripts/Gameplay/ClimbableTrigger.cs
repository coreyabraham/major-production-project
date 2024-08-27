using UnityEngine;
 
public class ClimbableTrigger : MonoBehaviour
{
<<<<<<< HEAD
 
    [field: SerializeField] private GameObject PipePoint;
    [field: Tooltip("The point in world space that the player will warp to when detaching from the pipe. Can be left empty if \"Use Ground Point\" is false.")]
    [field: SerializeField] private GameObject GroundPoint;
 
    [field: Tooltip("Determines if the \"Ground Point\" variable should be used. If set as false, then \"Ground Point\" can be left empty.")]
    [field: SerializeField] private bool useGroundPoint;
 
    
    private bool SkipJumpToClimbCheck = false;  // Used to disallow player to reattach to pipe they're already in the trigger of.
    private PlayerSystem playerSystem;
 
 
=======
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



>>>>>>> 41a1859ecf259f022af4a18e8508eaf5ca5d4b65
    private void DetermineClimbHook()
    {
        if (playerSystem.IsJumpingFromClimb && !SkipJumpToClimbCheck)
        {
            playerSystem.IsClimbing = true;
            playerSystem.IsJumpingFromClimb = false;
            playerSystem.SetVelocity(Vector3.zero);
<<<<<<< HEAD
 
=======

>>>>>>> 41a1859ecf259f022af4a18e8508eaf5ca5d4b65
            SkipJumpToClimbCheck = true;
        }
 
        if (!playerSystem.ClimbingRequested) return;
 
        playerSystem.IsClimbing = !playerSystem.IsClimbing;
        playerSystem.SetVelocity(Vector3.zero);
<<<<<<< HEAD
 
        if (playerSystem.IsClimbing)
        {
            playerSystem.Warp(PipePoint.transform.position);
=======

        if (playerSystem.IsClimbing)
        {
            if (!usePipePointYPos) { playerSystem.Warp(new(PipePoint.transform.position.x, playerSystem.transform.position.y, PipePoint.transform.position.z)); }
            else { playerSystem.Warp(PipePoint.transform.position); }

>>>>>>> 41a1859ecf259f022af4a18e8508eaf5ca5d4b65
            SkipJumpToClimbCheck = true;
        }
        else if (!playerSystem.IsClimbing && useGroundPoint)
        {
            playerSystem.Warp(GroundPoint.transform.position);
<<<<<<< HEAD
        }
=======

            playerSystem = null;
            SkipJumpToClimbCheck = false;
        }
        
>>>>>>> 41a1859ecf259f022af4a18e8508eaf5ca5d4b65
        playerSystem.ClimbingRequested = false;
    }
 
    private void Update()
    {
        if (!playerSystem) return;
 
        DetermineClimbHook();
 
        bool isOutsideBounds = false;
<<<<<<< HEAD
 
=======

        #region Buggy Pipe Detach
>>>>>>> 41a1859ecf259f022af4a18e8508eaf5ca5d4b65
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
<<<<<<< HEAD
 
=======
        #endregion

>>>>>>> 41a1859ecf259f022af4a18e8508eaf5ca5d4b65
        if (!isOutsideBounds) return;
        playerSystem.IsClimbing = false;
    }
 
 
    #region OnTrigger Functions
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerSystem = other.GetComponent<PlayerSystem>();
    }
<<<<<<< HEAD
 
=======


    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") || playerSystem == null) return;
        playerSystem = other.GetComponent<PlayerSystem>();
    }

>>>>>>> 41a1859ecf259f022af4a18e8508eaf5ca5d4b65
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (playerSystem.IsClimbing) { playerSystem.ClimbingRequested = true; }
        playerSystem = null;
<<<<<<< HEAD
 
=======

>>>>>>> 41a1859ecf259f022af4a18e8508eaf5ca5d4b65
        SkipJumpToClimbCheck = false;
    }
    #endregion
}