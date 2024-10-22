using UnityEngine;

public class MopHandleLaunch : MonoBehaviour
{
    public enum MopHandleStates
    {
        WaitForPlayer = 0,
        PlayerAttached,
        PlayerJumpFromMop,
        PlayerTimeout,
        Deactivated
    }

    #region Public Variables
    [field: Header("Mop Properties")]

    [field: Tooltip("The point in world space that the player will warp to when attaching to the Mop.")]
    [field: SerializeField] GameObject mopPoint;
    [field: Tooltip("How quickly the mop will build up speed when the player attaches to it.")]
    [field: SerializeField] float acceleration;

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
    bool doMopMove = false;
    Transform parentOfThis;
    PlayerSystem playSys;
    float rotateTimer = 0;
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) { return; }
        playSys = GameSystem.Instance.Player;
    }

    private void Update()
    {
        if (doMopMove)
        {
            if (rotateTimer < maxTime) { rotateTimer += Time.deltaTime; }

            float rotate = parentOfThis.rotation.z - 12 * (acceleration * (rotateTimer * rotateTimer));
            parentOfThis.rotation = Quaternion.Euler(parentOfThis.rotation.x, parentOfThis.rotation.y, rotate);
        }

        if (!playSys || attachState == MopHandleStates.Deactivated) { return; }

        if (attachState == MopHandleStates.WaitForPlayer)
        {
            playSys.IsClimbing = true;
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

            if (Input.GetKeyDown(KeyCode.Space) && rotateTimer <= maxTime) { attachState = MopHandleStates.PlayerJumpFromMop; }
            else if (rotateTimer > maxTime) { attachState = MopHandleStates.PlayerTimeout; }
        }

        if (attachState == MopHandleStates.PlayerJumpFromMop || attachState == MopHandleStates.PlayerTimeout)
        {
            playSys.gameObject.transform.parent = null;
            playSys.ToggleCharCont(true);
            playSys.IsClimbing = false;

            if (attachState == MopHandleStates.PlayerJumpFromMop)
            {
                float accuracy = Mathf.InverseLerp(0, targetTime, rotateTimer);
                if (accuracy < 0.1f) { accuracy = 0.1f; }

                playSys.ForcePlayerToJump(5.0f);
                playSys.ApplyImpulseToPlayer(accuracy);
                attachState = MopHandleStates.Deactivated;
            }
        }
    }

    private void Start()
    {
        parentOfThis = gameObject.transform.parent;
        //playSys = GameSystem.Instance.Player;
    }
}
