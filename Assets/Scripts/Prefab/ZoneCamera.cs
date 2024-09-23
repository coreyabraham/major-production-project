using UnityEngine;

public class ZoneCamera : MonoBehaviour, ITouchable
{
    // TODO: MOVE THIS ENUMERATION LATER!
    [System.Serializable]
    public enum LocalScaleUsage
    {
        X, Y, Z
    }

    [field: Header("ITouchable Inheritance")]
    [field: SerializeField] public bool Enabled { get; set; }

    [field: Header("CameraSystem Settings")]
    [field: SerializeField] private bool IgnoreAnticipationOffset;
    [field: SerializeField] private bool IgnoreCurrentOffset;

    [field: Header("Scaling Options")]
    [field: SerializeField] private LocalScaleUsage LocalScaleType = LocalScaleUsage.X;
    [field: SerializeField] private float TransformModifier = 0.5f;
    
    [field: Header("Utilization Options")]
    public bool UsePositionOffset;
    public bool UseRotationOffset;
    public bool UseFOVAdjustment;

    [field: Header("Overwriting Options")]
    public bool OverridePreviousPosition;
    public bool OverridePreviousRotation;
    public bool OverridePreviousFOV;

    [field: Header("Transformations")]
    public Vector3 TargetPosition;
    public Quaternion TargetRotation;
    [field: SerializeField] private Transform TargetOffsetObject;

    [field: Header("FOV Adjustments")]
    public float TargetFOV;
    [field: SerializeField] private bool DerivePreviousFOV;

    [field: Header("Camera Animations")]
    public bool ZoneBlending = false;
    public AnimationCurve LerpCurve;
    [field: SerializeField] private ZoneBlendType BlendType = ZoneBlendType.OffsetState;

    [field: Header("Miscellaneous")]
    [field: SerializeField] private bool HideOnStartup = false;

    [HideInInspector] public float TriggerWidth;

    private bool PreviousIAO;
    private bool PreviousICO;

    public void Entered(Collider other)
    {
        if (IgnoreAnticipationOffset)
        {
            PreviousIAO = IgnoreAnticipationOffset;
            GameSystem.Instance.Camera.IgnoreAnticipationOffset = false;
        }

        if (IgnoreCurrentOffset)
        {
            PreviousICO = IgnoreCurrentOffset;
            GameSystem.Instance.Camera.IgnoreCurrentOffset = false;
        }

        if (ZoneBlending) GameSystem.Instance.Camera.SetActiveTriggerZone(this);
        else GameSystem.Instance.Camera.GetNewZoneCameraOffsets(this);

        switch (BlendType)
        {
            case ZoneBlendType.OffsetState: GameSystem.Instance.Camera.SetCameraType(CameraType.OffsetState); break;
            case ZoneBlendType.TargetState: GameSystem.Instance.Camera.SetCameraType(CameraType.TargetState); break;
        }
    }

    public void Left(Collider other)
    {
        if (IgnoreAnticipationOffset) GameSystem.Instance.Camera.IgnoreAnticipationOffset = PreviousIAO;
        if (IgnoreCurrentOffset) GameSystem.Instance.Camera.IgnoreCurrentOffset = PreviousICO;

        if (ZoneBlending) GameSystem.Instance.Camera.SetActiveTriggerZone(null);
        else GameSystem.Instance.Camera.ResetZoneCamOffset();

        GameSystem.Instance.Camera.RevertCameraType();
    }

    private void Awake()
    {
        if (TargetOffsetObject != null)
        {
            switch (BlendType)
            {
                //Get offsets between target and TriggerZone 
                case ZoneBlendType.OffsetState: TargetPosition = TargetOffsetObject.transform.position - transform.position; break;
                case ZoneBlendType.TargetState: TargetPosition = TargetOffsetObject.transform.position; break;
            }

            //Get rotation of target and set in newRotOffset
            TargetRotation = TargetOffsetObject.transform.rotation;
        }

        if (TargetRotation.x > 180) { TargetRotation.x -= 360; }
        if (TargetRotation.y > 180) { TargetRotation.y -= 360; }
        if (TargetRotation.z > 180) { TargetRotation.z -= 360; }

        switch (LocalScaleType)
        {
            case LocalScaleUsage.X: TriggerWidth = transform.localScale.x * TransformModifier; break;
            case LocalScaleUsage.Y: TriggerWidth = transform.localScale.y * TransformModifier; break;
            case LocalScaleUsage.Z: TriggerWidth = transform.localScale.z * TransformModifier; break;
        }

        if (DerivePreviousFOV) TargetFOV = GameSystem.Instance.Camera.main.fieldOfView;

        if (!HideOnStartup) return;

        bool result = gameObject.TryGetComponent(out MeshRenderer renderer);
        if (!result) return;

        renderer.enabled = false;
    }
}
