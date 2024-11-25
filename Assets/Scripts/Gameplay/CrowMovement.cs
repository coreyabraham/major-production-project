using UnityEngine;

public class CrowMovement : MonoBehaviour
{
    #region Public Variables
    // Speed
    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;

    // Check for playing Death Animation
    public bool attackPlayer;
    #endregion


    #region Private Variables
    // Positions & Rotation
    private Vector3 targetDirection, originPosition;
    private Quaternion originRotation;

    // Collect Components
    private Rigidbody rb;
    private CrowAwareness awareness;

    // Reset Position on Death
    private float resetTimer = 0.0f;
    private bool resetTriggered = false;
    #endregion


    #region Private Functions
    private void FreezeRigidbody() => rb.constraints = RigidbodyConstraints.FreezeAll;
    private void UnfreezeRigidbody() => rb.constraints = RigidbodyConstraints.None;


    private void FixedUpdate()
    {
        if (!rb || !awareness) { return; }

        if (awareness.GetAwareness())
        {
            resetTriggered = true;

            // Rotate crow to face target.
            targetDirection = awareness.GetDirection();
            transform.LookAt(targetDirection);

            // Launch crow forward.
            rb.velocity = transform.forward * speed;
        }
        if (!resetTriggered) { return; }

        resetTimer += Time.fixedDeltaTime;

        if (resetTimer > 1f)
        {
            FreezeRigidbody();

            attackPlayer = true;
            rb.velocity = new(0, 0);
            transform.SetPositionAndRotation(originPosition, originRotation);

            UnfreezeRigidbody();

            attackPlayer = false;
            resetTimer = 0.0f;
            resetTriggered = false;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (!rb) { Debug.LogError("Script cannot make a reference to Rigidbody!"); }

        awareness = GetComponent<CrowAwareness>();
        if (!awareness) { Debug.LogError("Script cannot make a reference to CrowAwareness!"); }

        originPosition = transform.position;
        originRotation = transform.rotation;
    }
    #endregion
}