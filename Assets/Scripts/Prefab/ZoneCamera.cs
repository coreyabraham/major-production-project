using UnityEngine;

public class ZoneCamera : MonoBehaviour, ITouchable
{
    [field: Header("ITouchable Inheritance")]
    [field: SerializeField] public bool Enabled { get; set; }
    [field: SerializeField] public bool HideOnStartup { get; set; }

    [field: Header("CameraSystem Settings")]
    public bool IgnoreAnticipationOffset;
    public bool IgnoreCurrentOffset;

    [field: Header("Scaling Options")]
    public CartesianCoords LocalScaleType = CartesianCoords.X;
    [field: SerializeField] private float TransformModifier = 0.5f;
    
    [field: Header("Utilization Options")]
    public bool UsePositionOffset;
    public bool UseRotationOffset;
    public bool UseFOVAdjustment;

    [field: Header("Overriding Options")]
    public bool OverridePreviousPosition;
    public bool OverridePreviousRotation;
    public bool OverridePreviousFOV;

    [field: Header("Deriving Options")]
    [field: SerializeField] private bool DerivePosition;
    [field: SerializeField] private bool DeriveRotation;
    [field: SerializeField] private bool DeriveFieldOfView;

    [field: Header("Transformations")]
    public Vector3 TargetPosition;
    public Quaternion TargetRotation;
    [field: SerializeField] private Transform TargetObject;

    [field: Header("FOV Adjustments")]
    public float TargetFOV;
    [field: SerializeField] private bool SumDerivedFOV;

    [field: Header("Camera Animations")]
    public bool ZoneBlending = false;
    public AnimationCurve LerpCurve;
    [field: SerializeField] private ZoneBlendType BlendType = ZoneBlendType.OffsetState;

    [HideInInspector] public float TriggerSize;

    private bool PreviousIAO;
    private bool PreviousICO;

    public void Entered(PlayerSystem Player)
    {
        if (IgnoreAnticipationOffset)
        {
            PreviousIAO = GameSystem.Instance.Camera.IgnoreAnticipationOffset;
            GameSystem.Instance.Camera.IgnoreAnticipationOffset = true;
        }

        if (IgnoreCurrentOffset)
        {
            PreviousICO = GameSystem.Instance.Camera.IgnoreCurrentOffset;
            GameSystem.Instance.Camera.IgnoreCurrentOffset = true;
        }

        if (ZoneBlending) GameSystem.Instance.Camera.SetActiveTriggerZone(this);
        else GameSystem.Instance.Camera.GetNewZoneCameraOffsets(this);

        switch (BlendType)
        {
            case ZoneBlendType.OffsetState: GameSystem.Instance.Camera.SetCameraType(CameraType.OffsetState); break;
            case ZoneBlendType.TargetState: GameSystem.Instance.Camera.SetCameraType(CameraType.TargetState); break;
        }
    }

    public void Left(PlayerSystem Player)
    {
        if (IgnoreAnticipationOffset) GameSystem.Instance.Camera.IgnoreAnticipationOffset = PreviousIAO;
        if (IgnoreCurrentOffset) GameSystem.Instance.Camera.IgnoreCurrentOffset = PreviousICO;

        GameSystem.Instance.Camera.SetActiveTriggerZone(null);
        GameSystem.Instance.Camera.ResetZoneCamOffset();

        GameSystem.Instance.Camera.RevertCameraType();
    }

    public void Staying(PlayerSystem Player) { }

    private void Start()
    {
        if (TargetObject != null)
        {
            if (DerivePosition)
            {
                switch (BlendType)
                {
                    //Get offsets between target and TriggerZone 
                    case ZoneBlendType.OffsetState: TargetPosition = TargetObject.transform.position - transform.position; break;
                    case ZoneBlendType.TargetState: TargetPosition = TargetObject.transform.position; break;
                }
            }

            //Get rotation of target and set in newRotOffset
            if (DeriveRotation) TargetRotation = TargetObject.transform.rotation;
        }

        if (DeriveFieldOfView)
        {
            float originalTargetFOV = TargetFOV;
            TargetFOV = GameSystem.Instance.Camera.FieldOfView;
            if (SumDerivedFOV) TargetFOV += originalTargetFOV;
        }

        // TODO: This is part of the reason for bug #334's existence, please fix it!
        if (TargetRotation.x > 180) { TargetRotation.x -= 360; }
        if (TargetRotation.y > 180) { TargetRotation.y -= 360; }
        if (TargetRotation.z > 180) { TargetRotation.z -= 360; }

        switch (LocalScaleType)
        {
            case CartesianCoords.X: TriggerSize = transform.localScale.x * TransformModifier; break;
            case CartesianCoords.Y: TriggerSize = transform.localScale.y * TransformModifier; break;
            case CartesianCoords.Z: TriggerSize = transform.localScale.z * TransformModifier; break;
        }
    }

    private void Awake() => GetComponent<ITouchable>().SetupTrigger(gameObject);
}
