using UnityEngine;

public class ClutterClump : MonoBehaviour
{
    // Handles the clump itself, not the objects.

    #region Public Variables
    [field: Tooltip("How long the clump will hold together once the player touches it. Once this timer reaches its end, the clump will break apart.")]
    [field: SerializeField] private float holdDuration;
    [field: Tooltip("How long the clump will remain broken apart for until it reconstructs itself.")]
    [field: SerializeField] private float cooldownDuration;

    [field: Tooltip("Determines whether or not the parent GameObject will slowly move up and down to emulate buoyancy.")]
    [field: SerializeField] private bool emulateBuoyancy;


    Rigidbody rb;
    Vector3 pos;
    BoxCollider collision;
    bool buoyancyDir, setToBreak, isBroken;
    float buoyancyTimer, breakTimer, brokenTimer;
    AnimationCurve buoyancyCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    #endregion


    private void BreakClump()
    {
        collision.enabled = false;
        emulateBuoyancy = false;
        setToBreak = false;
        breakTimer = 0;

        // At the moment, enables rigidbody gravity and makes it sink.

        rb.useGravity = true;
        rb.isKinematic = false;

        isBroken = true;
    }


    private void RespawnClump()
    {
        rb.useGravity = false;
        rb.isKinematic = true;

        transform.position = pos;
        collision.enabled = true;
        emulateBuoyancy = true;
        isBroken = false;
        brokenTimer = 0;
    }


    private void OnTriggerEnter(Collider other)
    {
        // This method for detecting if the player is on the clump feels so scuffed.
        // There is a better way, but I spent an hour trying to get it to work with no success.

        if (other.CompareTag("Player"))
        {
            setToBreak = true;
        }
    }


    private void Update()
    {
        if (emulateBuoyancy)
        {
            buoyancyTimer += Time.deltaTime;

            var normalizedProgress = buoyancyTimer * (0.5f * 0.1f);
            var easing = buoyancyCurve.Evaluate(normalizedProgress);

            if (!buoyancyDir)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, pos + new Vector3(0, 0.06f, 0), easing);
                if (buoyancyTimer > 2.3) { buoyancyDir = true; buoyancyTimer = 0; }
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, pos, easing);
                if (buoyancyTimer > 2.3) { buoyancyDir = false; buoyancyTimer = 0; }
            }
        }

        if (setToBreak)
        {
            breakTimer += Time.deltaTime;
            if (breakTimer > holdDuration)
            {
                BreakClump();
            }
        }

        if (isBroken)
        {
            brokenTimer += Time.deltaTime;
            if (brokenTimer > cooldownDuration)
            {
                RespawnClump();
            }
        }
    }

    private void Start()
    {
        pos = transform.position;
        rb = GetComponent<Rigidbody>();
        collision = GetComponent<BoxCollider>();
    }
}
