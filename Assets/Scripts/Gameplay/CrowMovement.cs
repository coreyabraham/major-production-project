using UnityEngine;

public class CrowMovement : MonoBehaviour
{
    #region Public Variables
    // Speed
    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;

    // Check for playing Death Animation
    public bool attackPlayer;

    [field: Tooltip("Reference to the player model's Animator that's used to animate the character.")]
    public Animator animator;
    [field: Header("Collections")]
    [field: SerializeField] PlayerAnimation[] crowAnimations;
    #endregion


    #region Private Variables
    // Positions & Rotation
    private Vector3 targetDirection, originPosition;
    private Quaternion originRotation;

    // Collect Components
    private Rigidbody rb;
    private CrowAwareness awareness;

    // Reset Position on Death
    private float resetTimer = 0.0f, landingResetTimer = 0.0f;
    private bool resetTriggered = false;
    private bool IsLanding, IsFlying;
    #endregion


    #region Private Functions
    private void FreezeRigidbody() => rb.constraints = RigidbodyConstraints.FreezeAll;
    private void UnfreezeRigidbody() => rb.constraints = RigidbodyConstraints.None;


    private void FixedUpdate()
    {
        if (IsLanding)
        {
            landingResetTimer += Time.fixedDeltaTime;
            if (landingResetTimer >= 1)
            {
                IsLanding = false;
                IsFlying = false;
                landingResetTimer = 0.0f;
            }
        }

        if (!rb || !awareness) { return; }

        if (awareness.GetAwareness())
        {
            resetTriggered = true;
            IsFlying = true;

            // Rotate crow to face target.
            targetDirection = awareness.GetDirection();
            transform.LookAt(targetDirection);

            // Launch crow forward.
            rb.velocity = transform.forward * speed;
        }
        if (!resetTriggered) { return; }

        resetTimer += Time.fixedDeltaTime;

        if (resetTimer >= 0.8f) { IsFlying = false; IsLanding = true; }

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

            awareness.ResetAwareness();
        }
    }

    private void Update()
    {
        foreach (PlayerAnimation anim in crowAnimations)
        {
            if (anim.InternalAnimType != AnimType.Custom)
            {
                switch (anim.InternalAnimType)
                {
                    case AnimType.IsFlying: animator.SetBool(anim.Name, IsFlying); break;
                    case AnimType.IsLanding: animator.SetBool(anim.Name, IsLanding); break;
                }
                continue;
            }
            switch (anim.CustomValueType)
            {
                case AnimValueType.Boolean:
                    if (!bool.TryParse(anim.CustomValue, out bool resultBool)) return;
                    animator.SetBool(anim.Name, resultBool);
                    break;
            }
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