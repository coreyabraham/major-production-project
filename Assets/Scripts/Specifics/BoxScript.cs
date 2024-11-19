using UnityEngine;

public class BoxScript : MonoBehaviour
{
    #region Variables - Public
    [field: Header("Box Properties")]

    [field: Tooltip("The distance that the box should be from the player when grabbing.\n\nValues that are too low will cause the box to teleport over the player, while values that are too high will push it away too far.")]
    [field: SerializeField] float grabDistanceFromPlayer;
    #endregion


    Rigidbody rb;
    float timeout = 0.0f;
    bool doTimeout = false;


    #region Functions - Public
    public float GetGrabDistance() => grabDistanceFromPlayer / 10f;
    public void FreezeBox()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }
    public void UnfreezeBox()
    {
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        doTimeout = true;
    }



    private void Update()
    {
        if (!doTimeout) { return; }
        if (timeout < 1) { timeout += Time.deltaTime; return; }

        FreezeBox();

        doTimeout = false;
        timeout = 0.0f;
    }



    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (grabDistanceFromPlayer <= 0)
        {
            Debug.LogWarning("Properties for Grabbable Box have not been set correctly!");
            if (grabDistanceFromPlayer < 0) { grabDistanceFromPlayer = -grabDistanceFromPlayer; }
        }
    }
    #endregion
}
