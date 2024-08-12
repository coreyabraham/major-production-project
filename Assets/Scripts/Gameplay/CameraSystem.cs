using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [field: Header("Toggleables")]
    public bool TargetPlayer = false;

    [field: Header("Angles and Offsets")]
    [field: SerializeField] private Vector3 CameraPositionOffset;
    [field: SerializeField] private Vector3 CameraRotationOffset;

    [field: Space(5.0f)]

    [field: SerializeField] private float CameraAngle;
    [field: SerializeField] private float FieldOfView = 80.0f;
    [field: SerializeField] private float DepthOfField;

    [field: Header("Lerping")]
    [field: SerializeField] private bool LerpCamera;
    [field: SerializeField] private bool LerpSettings;

    [field: Space(5.0f)]

    [field: SerializeField] private float CameraLerpSpeed;
    [field: SerializeField] private float SettingsLerpSpeed;

    [field: Space(5.0f)]
    
    [field: SerializeField] private EasingStyle EasingStyle;
    [field: SerializeField] private EasingDirection EasingDirection;

    [field: Header("External References")]
    public PlayerSystem Player;

    [HideInInspector] public Camera main;

    private bool SkipAutoTargetSetting = false;
    private bool YieldMovementDuringSet = false;

    System.Action<CameraSystem> FinishedAction;
    MovementType PreviousMoveType;

    private Vector3 TargetPosition;
    private Quaternion TargetRotation;

    public void HookCameraToPlayer()
    {
        if (!SkipAutoTargetSetting) return;

        if (YieldMovementDuringSet) Player.SetMovementType(PreviousMoveType);

        YieldMovementDuringSet = false;
        SkipAutoTargetSetting = false;

        TargetPlayer = true;
    }

    public bool IsLerpingToPoint() => SkipAutoTargetSetting;

    public Transform GetCameraTransform() => main.transform;

    public void SetCameraTransform(Transform Transform, bool YieldMovement, System.Action<CameraSystem> Finished)
    {
        PreviousMoveType = Player.GetMoveType();
        SkipAutoTargetSetting = true;
        YieldMovementDuringSet = YieldMovement;

        if (YieldMovement) Player.SetMovementType(MovementType.None, true);

        TargetPosition = Transform.position;
        TargetRotation = Transform.rotation;

        FinishedAction = Finished;
    }

    public void ForceCameraBackToPlayer()
    {
        SkipAutoTargetSetting = true;

        Vector3 vecRot = new(main.transform.rotation.x, CameraAngle, main.transform.rotation.z);
        TargetRotation = Quaternion.Euler(vecRot + CameraRotationOffset);

        TargetPosition = Player.Character.gameObject.transform.position + CameraPositionOffset;

        HookCameraToPlayer();
    }

    private void LerpCameraTransform(Vector3 Position, Quaternion Rotation)
    {
        if (!LerpCamera)
        {
            main.transform.SetPositionAndRotation(Position, Rotation);
            return;
        }

        main.transform.SetPositionAndRotation(
            Vector3.MoveTowards(main.transform.position, Position, Time.fixedDeltaTime * CameraLerpSpeed),
            Quaternion.Lerp(main.transform.rotation, Rotation, Time.fixedDeltaTime * CameraLerpSpeed)
        );

        if (!SkipAutoTargetSetting) return;
        if (main.transform.position != Position || main.transform.rotation != Rotation) return;

        FinishedAction?.Invoke(this);
        FinishedAction = null;
    }

    //private void LerpCameraTransform(Transform Transform) => LerpCameraTransform(Transform.position, Transform.rotation);

    private void FixedUpdate()
    {
        if (LerpSettings)
        {
            main.fieldOfView = Mathf.Lerp(main.fieldOfView, Mathf.Clamp(FieldOfView, 0, 180), Time.fixedDeltaTime * SettingsLerpSpeed);
            //main.depthOfField = Mathf.Lerp(main.depthOfField, DepthOfField, Time.fixedDeltaTime * SettingsLerpSpeed);
        }
        else
        {
            main.fieldOfView = Mathf.Clamp(FieldOfView, 0, 180);
            //main.depthOfField = DepthOfField;
        }

        if (!SkipAutoTargetSetting && TargetPlayer)
        {
            Transform charTransform = Player.Character.gameObject.transform;
            Vector3 newPos = charTransform.position + CameraPositionOffset;

            Vector3 vecRot = new(main.transform.rotation.x, CameraAngle, main.transform.rotation.z);
            Quaternion newRot = Quaternion.Euler(vecRot + CameraRotationOffset);

            TargetPosition = newPos;
            TargetRotation = newRot;
        }

        LerpCameraTransform(TargetPosition, TargetRotation);
    }
    
    private void Awake() => main = GetComponentInChildren<Camera>();
}
