using UnityEngine;

public class temp_MopBucketLaunch : MonoBehaviour
{
    #region Public Variables
    [field: Tooltip("The point in world space that the player will warp to when attaching to the Mop.")]
    [field: SerializeField] private GameObject MopPoint;
    #endregion

    #region Private Variables
    Transform parentOfThis;
    PlayerSystem playSys;
    bool hasAttachedOnce = false;
    float timer = 0;
    #endregion



    void DoMopMove()
    {
        // Move and rotate the mop handle.
        // use "parent' for this

        timer += Time.deltaTime;


    }

    private void Update()
    {
        if (!playSys) { return; }

        if ((playSys.IsJumpingFromClimb || playSys.IsPlayerJumping()) && !hasAttachedOnce)
        {
            playSys.IsClimbing = true;
            playSys.IsJumpingFromClimb = false;
            playSys.SetMoveType(MoveType.None);
            playSys.SetVelocity(Vector3.zero);

            playSys.Warp(MopPoint.transform.position);
            playSys.gameObject.transform.parent = MopPoint.transform;

            hasAttachedOnce = true;
        }

        if (!playSys.ClimbingRequested) { return; }

        playSys.gameObject.transform.parent = null;
        // Put code to detach when jumping.
    }


    private void Start()
    {
        parentOfThis = gameObject.transform.parent;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) { return; }
        playSys = other.GetComponent<PlayerSystem>();
        DoMopMove();
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
