using UnityEngine;

#region Enums - Public
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
#endregion

public class PipeFunctionality : MonoBehaviour
{
    #region Variables - Public
    [field: Header("Pipe-Specific Variables")]

    [field: Tooltip("If true, the pipe automatically makes the player slide down. They will not be able to control themselves and will be forced to slide on this pipe. The only way off is to either jump or get detached at the bottom of the pipe.\n\nIf this is true, the other two Pipe-Specific Variables will be automatically set.")]
    [field: SerializeField] bool isPermaSlippery;
    [field: Tooltip("Does one end of this pipe connect to the ground?\nCan the player run into this pipe while grounded?")]
    [field: SerializeField] bool isConnectedToGround;
    [field: Tooltip("Does the player automatically detach from the pipe when they reach the bottom of it?")]
    [field: SerializeField] bool canDetachAtBottom;

    [field: Header("Player-Specific Variables")]

    [field: Tooltip("The side of the pipe that the player will attach to when interacting with this pipe.")]
    [field: SerializeField] PipeSide sideToAttachTo;
    [field: Tooltip("In case the incorrect axis is being used when attaching to the pipes, adjust them using this.")]
    [field: SerializeField] PipeAxis axis;
    #endregion

    #region Variables - Private
    private bool SkipJumpToClimbCheck = false;  // Prevents player from reattaching to current pipe if true.
    private float SkipJumpCooldown = 1;
    private PlayerSystem playSys;

    private BoxCollider Trig;
    #endregion

    #region Functions - Public
    public bool GetSlipperyState() => isPermaSlippery;
    #endregion
    
    #region Functions - Private
    private void ReferencePlayer()
    {
        playSys.CurrentPipe = this;
        playSys.CurrentPipeSide = sideToAttachTo;
        playSys.ToggleUpMovement(true);
        playSys.ToggleDownMovement(true);
    }


    private void DereferencePlayer()
    {
        playSys = null;
    }


    private void DetermineClimbHook()
    {
        if (!SkipJumpToClimbCheck)
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
        if (!playSys.IsPlayerJumping()) { return; }

        SkipJumpCooldown = 0;
        playSys.SetVelocity(Vector3.zero);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.root.CompareTag("Player")) return;
        playSys = other.GetComponent<PlayerSystem>();
        ReferencePlayer();
    }


    private void Update()
    {
        if (SkipJumpCooldown < 0.2f)
        {
            SkipJumpCooldown += Time.deltaTime;
            SkipJumpToClimbCheck = true;
            DereferencePlayer();
        }
        else
        {
            SkipJumpToClimbCheck = false;
        }

        if (!playSys) return;

        if (playSys.gameObject.transform.position.y - 0.02f > Trig.bounds.max.y) { playSys.ToggleUpMovement(false); }
        if (playSys.gameObject.transform.position.y - 0.23f < Trig.bounds.min.y && !isConnectedToGround)
        {
            if (canDetachAtBottom)
            {
                playSys.IsClimbing = false;
                DereferencePlayer();
                return;
            }
            else
            {
                playSys.ToggleDownMovement(false);
            }
        }

        DetermineClimbHook();
    }


    private void Awake()
    {
        Trig = GetComponent<BoxCollider>();
        if (isConnectedToGround) { canDetachAtBottom = true; }

        if (isPermaSlippery) { isConnectedToGround = false; canDetachAtBottom = true; }
    }
    #endregion
}