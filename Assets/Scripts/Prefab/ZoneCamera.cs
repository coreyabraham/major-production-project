using UnityEngine;

public class ZoneCamera : MonoBehaviour, ITouchable
{
    // Offset State = Use Target Position and Rotation mixed with the "Target Offset Object" (Use `GetCameraOffset() and `SetCameraOffset()`)
    // Target State = Directly use "Target Offset Object" Transform(Manually set Camera Transform)

    // TODO: MOVE THIS ENUMERATION LATER!
    [System.Serializable]
    public enum LocalScaleUsage
    {
        LocalScaleX,
        LocalScaleY,
        LocalScaleZ
    }

    [field: Header("ITouchable Inheritance")]
    [field: SerializeField] public bool Enabled { get; set; }

    [field: Header("Scaling Options")]
    [field: SerializeField] private LocalScaleUsage LocalScaleType;
    
    [field: Header("Utilization Options")]
    [field: SerializeField] private bool UsePositionOffset;
    [field: SerializeField] private bool UseRotationOffset;
    [field: SerializeField] private bool UseFOVAdjustment;

    [field: Header("Overwriting Options")]
    [field: SerializeField] private bool OverridePreviousPosition;
    [field: SerializeField] private bool OverridePreviousRotation;
    [field: SerializeField] private bool OverridePreviousFOV;

    [field: Header("Transformations")]
    [field: SerializeField] private Vector3 TargetPosition;
    [field: SerializeField] private Quaternion TargetRotation;
    [field: SerializeField] private Transform TargetOffsetObject;

    [field: Header("FOV Adjustments")]
    [field: SerializeField] private float TargetFOV;
    [field: SerializeField] private bool DerivePreviousFOV;

    [field: Header("Camera Animations")]
    [field: SerializeField] private bool ZoneBlending = false;
    [field: SerializeField] private AnimationCurve LerpCurve;
    [field: SerializeField] private ZoneBlendType BlendType = ZoneBlendType.OffsetState;

    [field: Header("Miscellaneous")]
    [field: SerializeField] private bool HideOnStartup = false;

    private bool PlayerIsInTrigger = false;

    private bool PreviousSeparateOffsets;
    private bool PreviousPreviousAnticipationOffset;
    private CameraType PreviousCameraType;

    private CameraTarget PreviousOffset;

    private PlayerSystem Player;
    private CameraSystem Camera;

    public void Entered(Collider other)
    {
        PlayerIsInTrigger = true;

        PreviousSeparateOffsets = Camera.SeparateOffsets;
        PreviousPreviousAnticipationOffset = Camera.IgnoreAnticipationOffset;
        PreviousCameraType = Camera.GetCameraType();

        Camera.SeparateOffsets = TargetOffsetObject != null;
        Camera.IgnoreAnticipationOffset = true;
        Camera.SetCameraType(CameraType.Scriptable);

        PreviousOffset = Camera.GetCameraOffset();

        NoBlendingSet();
    }

    public void Left(Collider other)
    {
        PlayerIsInTrigger = false;

        Camera.SetCameraType(PreviousCameraType);

        Camera.SeparateOffsets = PreviousSeparateOffsets;
        Camera.IgnoreAnticipationOffset = PreviousPreviousAnticipationOffset;

        Camera.SetCameraOffset(
            !OverridePreviousPosition ? PreviousOffset.position : TargetPosition,
            !OverridePreviousRotation ? PreviousOffset.rotation : TargetRotation
        );
    }

    private void OnEnable()
    {
        if (!TargetOffsetObject) return;
        TargetPosition = transform.position - TargetOffsetObject.transform.position;
        TargetRotation = TargetOffsetObject.transform.rotation;
    }

    private void NoBlendingSet()
    {
        // This may need to be optimized in the future, for now, this should do for non-zone blending functionality!
        // I'm not trying to become the next Yandare Dev, we should all take a note out of that for what NOT to do during C# scripting.

        if (TargetOffsetObject != null)
        {
            Camera.SetCameraOffset(TargetOffsetObject.transform);
            return;
        }

        if (UsePositionOffset && !UseRotationOffset) Camera.SetCameraOffset(TargetPosition);
        else if (!UsePositionOffset && UseRotationOffset) Camera.SetCameraOffset(TargetRotation);
        else if (UsePositionOffset && UseRotationOffset) Camera.SetCameraOffset(TargetPosition, TargetRotation);
    }

    private void FixedUpdate()
    {
        if (!PlayerIsInTrigger) return;
        if (!ZoneBlending) return;

        // Add "Use X" and "Use Y" Localscale options in FixedUpdate() at runtime specifically
        float distance = Mathf.Abs(Player.gameObject.transform.position.x - transform.position.x);

        float clamp = distance / (transform.localScale.x / 2);
        float value = LerpCurve.Evaluate(clamp);

        print(value);

        Transform current = Camera.GetCameraTransform();

        //Camera.LerpCameraTransform(
        //    TargetOffsetObject.transform.position, 
        //    Time.fixedDeltaTime * value
        //);

        Camera.transform.position = Vector3.Lerp(current.position, TargetOffsetObject.transform.position, value);
    }

    private void Start()
    {
        Camera = GameSystem.Instance.Camera;
        Player = GameSystem.Instance.Player;
    }

    private void Awake()
    {
        for (int i = 0; i < LerpCurve.keys.Length; i++)
        {
            var key = LerpCurve.keys[i];

            print("Tangent: " + key.inTangent.ToString());
            print("InWeight: " + key.inWeight.ToString());
            print("OutTangent: " + key.outTangent.ToString());
            print("OutWeight: " + key.outWeight.ToString());
            print("Time: " + key.time.ToString());
            print("Value: " + key.value.ToString());
            print("Weighted Mode: " + key.weightedMode.ToString());
            print("=======================");
        }

        if (!HideOnStartup) return;

        bool result = gameObject.TryGetComponent(out MeshRenderer renderer);
        if (!result) return;

        renderer.enabled = false;
    }
}
