using UnityEngine;

public class BoxScript : MonoBehaviour
{
    #region Variables - Public
    [field: Header("Box Properties")]

    [field: Tooltip("The distance that the box should be from the player when grabbing.\n\nValues that are too low will cause the box to teleport over the player, while values that are too high will push it away too far.")]
    [field: SerializeField] float grabDistanceFromPlayer;

    [field: SerializeField] ParticleSystem rightParticleSystem;
    [field: SerializeField] ParticleSystem leftParticleSystem;

    [field: SerializeField] int rightEmissionRate = 20;
    [field: SerializeField] int leftEmissionRate = 20;
    #endregion

    #region Variables - Private
    Rigidbody rb;
    float timeout = 0.0f;
    bool doTimeout = false;

    public float particleTimer = 1.0f;
    float timer = 0.0f;

    float previousXPos;
    float marginOfError = 0.01f;

    #endregion

    #region Functions - Public
    public float GetGrabDistance() => grabDistanceFromPlayer / 10f;
    public void FreezeBox()
    {
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }
    public void UnfreezeBox()
    {
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        doTimeout = true;
    }


    private void Update()
    {
        timer += Time.deltaTime;
        if(particleTimer < timer && rightParticleSystem != null && leftParticleSystem !=null)
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
            timer = 0;
        }
        else { Debug.LogError("Particle Systems have not been set"); }

        if (!doTimeout) { return; }
        if (timeout < 0.1f) { timeout += Time.deltaTime; return; }

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