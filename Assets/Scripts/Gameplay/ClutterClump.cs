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
    bool buoyancyDir, setToBreak, isBroken;
    float buoyancyTimer, breakTimer;
    AnimationCurve buoyancyCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    #endregion


    private void BreakClump()
    {
        setToBreak = false;
        breakTimer = 0;

        // At the moment, enables rigidbody gravity.

        rb.useGravity = true;
        rb.isKinematic = false;
    }



    private void OnCollisionEnter(Collision other)
    {
        // Detect player collision

        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player is touching clump");
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
    }

    private void Start()
    {
        pos = transform.position;
        rb = GetComponent<Rigidbody>();
    }
}
