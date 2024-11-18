using UnityEngine;

public class MopHandleLaunch : MonoBehaviour
{
    public enum MopHandleStates
    {
        WaitForPlayer = 0,
        PlayerAttached,
        PlayerJumpFromMop,
        PlayerTimeout,
        Rewind,
        Deactivated
    }

    #region Public Variables
    [field: Header("Mop Properties")]

    [field: Tooltip("The point in world space that the player will warp to when attaching to the Mop.")]
    [field: SerializeField] GameObject mopPoint;
    [field: Tooltip("How quickly the mop will build up speed when the player attaches to it.")]
    [field: SerializeField] float acceleration;
    [field: Tooltip("Should the mop automatically reset itself after two seconds?")]
    [field: SerializeField] bool resetMopAfterJump = true;

    [field: Header("Target Values")]

    [field: Tooltip("The maximum amount of time the player has before they are automatically detached from the mop.")]
    [field: SerializeField] float maxTime;
    [field: Tooltip("The exact time in seconds that the player must wait for before they need to jump from the mop handle.")]
    [field: SerializeField] float targetTime;
    [field: Tooltip("The amount of force applied to the player when they jump from the mop handle. Treat this value as what will be applied after perfectly timing the jump, since this value will decrease automatically the further off \"Target Time\" it is.")]
    [field: SerializeField] float targetForce;

    [field: Header("Debug Variables")]

    [field: SerializeField] MopHandleStates attachState;
    #endregion

    #region Private Variables
    bool doMopMove = false, requireButtonRelease = false;
    float rotateTimer = 0, rewindTimer = 0;
    Transform parentOfThis;

    PlayerSystem playSys;
    
    Quaternion mopRotOrigin;
    #endregion

    #region Public Functions
    public void ResetMopBucket()
    {
        playSys = null;
        doMopMove = false;
        parentOfThis.rotation = mopRotOrigin;
        rotateTimer = 0;
        rewindTimer = 0;
        attachState = MopHandleStates.WaitForPlayer;
    }
    #endregion

    #region Private Functions
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) { return; }
        playSys = GameSystem.Instance.Player;

        if (playSys.IsJumping) { requireButtonRelease = true; }
    }


    private void Update()
    {
        if (doMopMove)
        {
            if (rotateTimer < maxTime) { rotateTimer += Time.deltaTime; }

            float rotate = parentOfThis.rotation.z - 12 * (acceleration * (rotateTimer * rotateTimer));
            parentOfThis.rotation = Quaternion.Euler(parentOfThis.rotation.x, parentOfThis.rotation.y, rotate);
        }

        if (attachState == MopHandleStates.Rewind && resetMopAfterJump)
        {
            rewindTimer += Time.deltaTime;
            if (rewindTimer >= 1.8f) { ResetMopBucket(); }
        }

        if (!playSys) { return; }

        if (attachState == MopHandleStates.WaitForPlayer)
        {
            playSys.IsOnMop = true;
            playSys.IsJumpingFromClimb = false;
            playSys.SetMoveType(MoveType.None, true);

            playSys.Warp(mopPoint.transform.position);
            playSys.gameObject.transform.parent = mopPoint.transform;

            attachState = MopHandleStates.PlayerAttached; return;
        }

        if (attachState == MopHandleStates.PlayerAttached)
        {
            doMopMove = true;

            if (playSys.WarpPosition == Vector3.zero && playSys.Character.enabled)
            {
                playSys.ToggleCharCont(false);
                playSys.SetMoveType(MoveType.TwoDimensionsOnly);
            }

            if (!playSys.IsJumping) { requireButtonRelease = false; }
            if (requireButtonRelease) { return; }

            if (playSys.JumpingRequested && rotateTimer <= maxTime) { attachState = MopHandleStates.PlayerJumpFromMop; }
            else if (rotateTimer > maxTime) { attachState = MopHandleStates.PlayerTimeout; }
        }

        if (attachState == MopHandleStates.PlayerJumpFromMop || attachState == MopHandleStates.PlayerTimeout)
        {
            playSys.gameObject.transform.parent = null;
            playSys.ToggleCharCont(true);
            playSys.IsOnMop = false;

            if (attachState == MopHandleStates.PlayerJumpFromMop)
            {
                float accuracy = Mathf.InverseLerp(0, targetTime, rotateTimer);
                if (accuracy < 0.1f) { accuracy = 0.1f; }

                playSys.ForcePlayerToJump(5.0f);
                playSys.LaunchPlayerFromMopBucket(accuracy);
            }

            attachState = MopHandleStates.Rewind;
        }
    }

    private void Start()
    {
        parentOfThis = gameObject.transform.parent;
        mopRotOrigin = parentOfThis.rotation;
    }
    #endregion
}
