using UnityEngine;

public class MopHandleLaunch : MonoBehaviour
{
    #region Public Variables
    [field: Tooltip("The point in world space that the player will warp to when attaching to the Mop.")]
    [field: SerializeField] GameObject mopPoint;
    [field: Tooltip("Initiates the rotation when the rat attaches to it. Used for launching them from the mop.")]
    [field: SerializeField] bool doMopMove = false;

    [field: Header("Target Values")]

    [field: Tooltip("The exact time in seconds that the player must wait for before they need to jump from the mop handle.")]
    [field: SerializeField] float targetTime;
    [field: Tooltip("The amount of force applied to the player when they jump from the mop handle. Treat this value as what will be applied after perfectly timing the jump, since this value will decrease automatically the further off \"Target Time\" it is.")]
    [field: SerializeField] float targetForce;
    #endregion

    #region Private Variables
    Transform parentOfThis;
    PlayerSystem playSys;
    CharacterController Character;
    bool hasAttachedOnce = false;
    float rotateTimer = 0;
    #endregion



    private void Update()
    {
        if (doMopMove)
        {
            rotateTimer += Time.deltaTime;

            parentOfThis.rotation = Quaternion.Euler(parentOfThis.rotation.x, parentOfThis.rotation.y, parentOfThis.rotation.z - 12 * (rotateTimer * rotateTimer));
        }

        if (!playSys) { return; }

        if (!hasAttachedOnce)
        {
            doMopMove = true;
            playSys.IsClimbing = true;
            playSys.IsJumpingFromClimb = false;
            playSys.SetMoveType(MoveType.None);
            playSys.SetVelocity(Vector3.zero);

            playSys.Warp(mopPoint.transform.position);
            playSys.gameObject.transform.parent = mopPoint.transform;

            // Using reference to playSys, call ToggleCharCont(false).
            // Re-enable it on jump from the mop head.

            hasAttachedOnce = true;
        }

        if (!playSys.ClimbingRequested) { return; }

        playSys.gameObject.transform.parent = null;
        // Put code to detach when jumping.

        // If (player jumps && rotateTimer is near TargetTime), targetForce is *1
        // If (player jumps && rotateTimer is not near TargetTime), targetForce is *0.5
        // If (player jumps && rotateTimer is way off TargetTime), targetForce is *0.1
    }


    private void Start()
    {
        parentOfThis = gameObject.transform.parent;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) { return; }
        playSys = other.GetComponent<PlayerSystem>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") || playSys) { return; }
        playSys = other.GetComponent<PlayerSystem>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) { return; }
        playSys = null;
    }
}
