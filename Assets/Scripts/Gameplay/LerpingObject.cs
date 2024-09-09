using UnityEngine;


// Used for lerping object between two points in world space.
public class LerpingObject : MonoBehaviour
{
    #region Public Variables
    [field: Header("World Space Coords")]

    [field: Tooltip("The point in world space where the trigger will begin. Once reached, it will lerp to \"endPos\".\n\nIf left as (0, 0, 0), this variable will automatically be set to the GameObject's current position on start.")]
    [field: SerializeField] Vector3 startPos;
    [field: Tooltip("The point in world space where the trigger will end. Once reached, it will lerp to \"startPos\".")]
    [field: SerializeField] Vector3 endPos;

    [field: Header("Timing & Smoothing")]

    [field: Tooltip("The speed at which movement from one point to the other should be.\n\nNote that providing a number that's too big may result in lerps to points being interrupted before they finish.")]
    [field: SerializeField] float moveTimeMultiplier;
    [field: Tooltip("The duration of time that the object will pause for before it begins lerping to its opposite point.")]
    [field: SerializeField] float pauseTime;
    [field: Tooltip("The smoothing that will be applied to the object's movement. By default, it eases in and out.")]
    [field: SerializeField] AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    #endregion

    #region Private Variables
    bool dir;
    float timeElapsed = 0;
    #endregion

    private void Update()
    {
        timeElapsed += Time.deltaTime;

        var normalizedProgress = timeElapsed * (moveTimeMultiplier * 0.1f);
        var easing = curve.Evaluate(normalizedProgress);

        if (!dir)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, endPos, easing);
            if (timeElapsed > pauseTime) { dir = true; timeElapsed = 0; } 
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, easing);
            if (timeElapsed > pauseTime) { dir = false; timeElapsed = 0; }
        }
    }

    private void Start()
    {
        startPos = transform.localPosition;
    }
}
