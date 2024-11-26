using UnityEngine;

public class BoxScript : MonoBehaviour
{
    #region Variables - Public
    [field: Header("Box Properties")]

    [field: Tooltip("The distance that the box should be from the player when grabbing.\n\nValues that are too low will cause the box to teleport over the player, while values that are too high will push it away too far.")]
    [field: SerializeField] float grabDistanceFromPlayer;

    [field: Header("Particle Properties")]

    [field: SerializeField] ParticleSystem rightParticleSystem;
    [field: SerializeField] ParticleSystem leftParticleSystem;

    [field: Space]

    [field: SerializeField] int rightEmissionRate = 20;
    [field: SerializeField] int leftEmissionRate = 20;

    [field: Space]

    [field: SerializeField] float particleTimer = 0.2f;
    #endregion

    #region Variables - Private
    Rigidbody rb;
    float grabTimeout = 0.0f;
    bool doTimeout = false, canUseParticles = false;

    float particleTimerDelay = 0.0f;
    float previousXPos;
    float marginOfError = 0.01f;

    RaycastHit ray;
    #endregion

    #region Functions - Public
    public float GetGrabDistance() => grabDistanceFromPlayer / 10f;
    public bool CheckIfValidPush(Vector3 direction)
    {
        // Draw ray.
        return true;
    }
    public void FreezeBox() => rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    public void UnfreezeBox()
    {
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        doTimeout = true;
    }
    #endregion

    #region Functions - Private
    private void Update()
    {
        // Particle stuff. Is ignored if not set correctly in the inspector.
        if (canUseParticles) { particleTimerDelay += Time.deltaTime; }

        if (canUseParticles && particleTimer < particleTimerDelay)
        {
            if(previousXPos - marginOfError >= gameObject.transform.position.x)
            {
                //Going Left
                var emission = leftParticleSystem.emission;
                emission.rateOverTime = leftEmissionRate;

                var otherEmission = rightParticleSystem.emission;
                otherEmission.rateOverTime = 0;
            }
            else if (gameObject.transform.position.x >= previousXPos + marginOfError)
            {
                //Going Right
                var emission = rightParticleSystem.emission;
                emission.rateOverTime = rightEmissionRate;

                var otherEmission = leftParticleSystem.emission;
                otherEmission.rateOverTime = 0;
            }
            else 
            {
                //Stationary
                var emission = leftParticleSystem.emission;
                emission.rateOverTime = 0;

                var otherEmission = rightParticleSystem.emission;
                otherEmission.rateOverTime = 0;
            }
            previousXPos = gameObject.transform.position.x;
            particleTimerDelay = 0;
        }

        if (!doTimeout) { return; }
        if (grabTimeout < 0.1f) { grabTimeout += Time.deltaTime; return; }

        FreezeBox();

        doTimeout = false;
        grabTimeout = 0.0f;
    }



    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (grabDistanceFromPlayer <= 0)
        {
            Debug.LogWarning("Properties for Grabbable Box have not been set correctly!");
            if (grabDistanceFromPlayer < 0) { grabDistanceFromPlayer = -grabDistanceFromPlayer; }
        }

        if (rightParticleSystem != null && leftParticleSystem != null) { canUseParticles = true; }
        else { Debug.LogWarning("One or both particles have not been set. They must both be set in order to use particles."); }
    }
    #endregion
}