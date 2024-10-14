using UnityEngine;

public enum PipeSide
{
    Left = 0,
    Right
}

public enum PipeAxis
{
    X = 0,
    Z
}

public class PipeFunctionality : MonoBehaviour
{
    /* Things that are needed:
     * - Why is this so terrible.
     * - Only allow jump when pushing in the same direction as their current up-direction, which will be away from the pipe.
     * - Store max Y value of trigger. Prevent up movement on pipe when the player is above this range.
    */

    [field: Header("Pipe Specifics")]

    [field: Tooltip("The side of the pipe that the player will attach to when interacting with this pipe.")]
    [field: SerializeField] PipeSide sideToAttachTo;
    [field: Tooltip("In case the incorrect axis is being used when attaching to the pipes, adjust them using this.")]
    [field: SerializeField] PipeAxis axis;

    private bool PlayerIsOn = false, SkipJumpToClimbCheck = false;  // Prevents player from reattaching to current pipe if true.
    private float SkipJumpCooldown = 1;
    private PlayerSystem playSys;


    private void DetermineClimbHook()
    {
        if (!SkipJumpToClimbCheck && !playSys.IsPlayerGrounded())
        {
            playSys.IsClimbing = true;
            playSys.IsJumpingFromClimb = false;
            playSys.SetVelocity(Vector3.zero);

            float climbOffset;
            if (sideToAttachTo == PipeSide.Left)
            {
                // Rotate player.
                if (axis == PipeAxis.X) { climbOffset = transform.position.x - 0.1f; }
                else { climbOffset = transform.position.z - 0.1f; }
            }
            else
            {
                // Rotate player.
                if (axis == PipeAxis.X) { climbOffset = transform.position.x + 0.1f; }
                else { climbOffset = transform.position.z + 0.1f; }
            }

            if (axis == PipeAxis.X) { playSys.Warp(new(climbOffset, playSys.transform.position.y, playSys.transform.position.z)); }
            else { playSys.Warp(new(playSys.transform.position.x, playSys.transform.position.y, climbOffset)); }
        }


        // Beyond the below line, they've requested (as in pushed the interact key/button), which will detach them.
        //if (!playSys.ClimbingRequested) return;
        if (!Input.GetKeyDown(KeyCode.Space)) { return; }

        SkipJumpCooldown = 0;
        playSys.SetVelocity(Vector3.zero);
    }


    #region Functions - Private
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playSys = other.GetComponent<PlayerSystem>();
        PlayerIsOn = true;
    }


    private void Update()
    {
        if (SkipJumpCooldown < 0.5f) { SkipJumpCooldown += Time.deltaTime; SkipJumpToClimbCheck = true; PlayerIsOn = false; playSys = null; }
        else { SkipJumpToClimbCheck = false; }

        if (!playSys) return;
        DetermineClimbHook();
    }
    #endregion
}