using UnityEngine;


// This script only handles the moving aspect of the trigger, NOT the hazard itself.
public class MovingHazardTrigger : MonoBehaviour
{
    #region Public Variables
    [field: Header("World Space Coords")]

    [field: Tooltip("The point in world space where the trigger will begin. Once reached, it will lerp to \"endPos\".\n\nThis variable will automatically be set to the GameObject's position on start.")]
    [field: SerializeField] Vector3 startPos;
    [field: Tooltip("The point in world space where the trigger will end. Once reached, it will lerp to \"startPos\".")]
    [field: SerializeField] Vector3 endPos;

    [field: SerializeField] float totalMoveTime;
    [field: SerializeField] AnimationCurve curve;
    #endregion

    #region Private Variables
    bool dir;
    float timeElapsed = 0;
    #endregion

    private void Update()
    {
        timeElapsed += Time.deltaTime;

        var normalizedProgress = timeElapsed / totalMoveTime;
        var easing = curve.Evaluate(normalizedProgress);

        if (!dir)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, endPos, easing);
            if (timeElapsed > 3) { dir = true; timeElapsed = 0; } 
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, easing);
            if (timeElapsed > 3) { dir = false; timeElapsed = 0; }
        }
    }

    private void Start()
    {
        startPos = transform.localPosition;
    }
}
