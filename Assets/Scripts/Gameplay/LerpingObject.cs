using UnityEngine;


// Used for lerping object between two points in world space.
public class LerpingObject : MonoBehaviour, ITouchable
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

    [field: Header("Bools")]

    [field: Tooltip("Determine whether or not the Player should become a child of this object when it comes into contact with it. Necessary if this script is applied to moving platforms that the Player will be standing on.")]
    [field: SerializeField] bool parentToPlayer;

    [field: Tooltip("Use `FixedUpdate()` over regular `Update()` functionality, useful for when you want this object to move in sync with Player Movement.")]
    [field: SerializeField] bool useFixedUpdate;

    [field: Header("ITouchable Inherited")]
    [field: SerializeField] public bool Enabled { get; set; } = true;
    [field: SerializeField] public bool HideOnStartup { get; set; } = false;
    #endregion

    #region Private Variables
    bool dir;
    float timeElapsed = 0;
    Transform previousPlayerParent;
    #endregion

    private void Callback(float DeltaTime)
    {
        timeElapsed += DeltaTime;

        var normalizedProgress = timeElapsed * (moveTimeMultiplier * 0.1f);
        var easing = curve.Evaluate(normalizedProgress);

        transform.localPosition = Vector3.Lerp(transform.localPosition, dir ? startPos : endPos, easing);

        if (timeElapsed < pauseTime) return;

        dir = !dir;
        timeElapsed = 0.0f;
    }

    private void Update()
    {
        if (useFixedUpdate) return;
        Callback(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!useFixedUpdate) return;
        Callback(Time.fixedDeltaTime);
    }

    private void Start()
    {
        if (startPos == Vector3.zero) 
            startPos = transform.localPosition;
        
        transform.position = startPos;
    }

    public void Entered(Collider other)
    {
        if (!parentToPlayer) return;
        if (previousPlayerParent != null) return;

        previousPlayerParent = GameSystem.Instance.Player.transform.parent;
        GameSystem.Instance.Player.transform.SetParent(transform);
    }

    public void Left(Collider other)
    {
        if (!parentToPlayer) return;
        GameSystem.Instance.Player.transform.SetParent(previousPlayerParent);
        previousPlayerParent = null;
    }
}
