using UnityEngine;

public class CrowAwareness : MonoBehaviour
{
    #region Public Variables
    [SerializeField] HazardSounds hazardTrigger;
    [SerializeField] float awarenessDistance;
    #endregion


    #region Private Variables
    private bool AwareOfPlayer;
    private Transform playerTransform;
    #endregion


    #region Public Functions
    public bool GetAwareness() => AwareOfPlayer;
    public void ResetAwareness() => AwareOfPlayer = false;
    public Vector3 GetDirection() => playerTransform.position;
    #endregion


    #region Private Functions
    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (!hazardTrigger) { Debug.Log("Hazard Trigger has not been set in the Inspector!"); }
    }

    private void Update()
    {
        if (!hazardTrigger) { return; }
        if (!hazardTrigger.hasBeenSpotted) { return; }
        AwareOfPlayer = playerTransform.position.magnitude <= awarenessDistance;
    }
    #endregion
}