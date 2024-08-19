using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [field: Header("Toggleables")]
    public bool TargetPlayer = false;

    [field: Header("Angles and Offsets")]
    [field: SerializeField] private CameraTarget Offsets;
    private CameraTarget PreviousOffsets;
    private CameraTarget DefaultOffsets;

    [field: Space(5.0f)]

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
    [field: SerializeField, Tooltip("Not Implemented Yet!")] private EasingDirection EasingDirection;

    [field: Header("External References")]
    public PlayerSystem Player;

    [HideInInspector] public Camera main;

    private bool SkipAutoTargetSetting = false;
    private bool YieldMovementDuringSet = false;

    MovementType PreviousMoveType;
    MovementType DefaultMoveType;

    private CameraTarget CurrentTarget;

    private bool MovingBetweenPoints = false;
    private int CamPointsMovedTo = 0;
    private int MaxCamPoints = 0;

    private float CurrentTime = 0.0f;
    private float MaxTime = 0.0f;
    private float ModifierTime = 0.0f;

    private System.Action<CameraSystem> FinishedAction;
    private System.Action TimeIntervalReached;

    public bool IsLerpingToPoint() => SkipAutoTargetSetting;
    public Transform GetCameraTransform() => main.transform;

    public void HookCameraToPlayer()
    {
        if (!SkipAutoTargetSetting) return;
        if (YieldMovementDuringSet) Player.SetMovementType(PreviousMoveType);

        YieldMovementDuringSet = false;
        SkipAutoTargetSetting = false;

        TargetPlayer = true;
        PreviousMoveType = MovementType.None;
    }

    public void ForceCameraBackToPlayer()
    {
        SkipAutoTargetSetting = true;
        CurrentTarget = GetPlayerCamPositionAndRotation();
    }

    private void FinishedMovementToTarget(CameraTarget Target)
    {
        print(Target.Position);
        print(Target.Rotation);

        if (MovingBetweenPoints && CamPointsMovedTo < MaxCamPoints)
        {
            CamPointsMovedTo++;
            return;
        }

        MovingBetweenPoints = false;
        CamPointsMovedTo = 0;
        MaxCamPoints = 0;

        FinishedAction?.Invoke(this);

        FinishedAction = null;
        TimeIntervalReached = null;
    }

    public CameraTarget GetCameraOffset() => Offsets;
    public void RevertCameraOffsets() => Offsets = PreviousOffsets;
    public void SetToDefaultOffsets() => Offsets = DefaultOffsets;

    public void SetCameraOffsets(CameraTarget Target) => SetCameraOffsets(Target.Position, Target.Rotation);
    public void SetCameraOffsets(Transform Target) => SetCameraOffsets(Target.position, Target.rotation);
    public void SetCameraOffsets(Vector3 Position, Quaternion Rotation)
    {
        PreviousOffsets = new()
        {
            Position = Position,
            Rotation = Rotation
        };

        Offsets.Position = Position;
        Offsets.Rotation = Rotation;
    }

    public void SetMultipleCameraTargets(CameraTarget[] Targets, bool YieldMovement, float MaxIntervalBetweenEach, float IntervalModifier, System.Action<CameraSystem> Finished)
    {
        if (MovingBetweenPoints) return;

        void TimeIntervalMet() => SetCameraTarget(Targets[CamPointsMovedTo], YieldMovement, Finished);

        MovingBetweenPoints = true;
        MaxCamPoints = Targets.Length;
        CamPointsMovedTo = 0;

        MaxTime = MaxIntervalBetweenEach;
        ModifierTime = IntervalModifier;

        TimeIntervalReached = TimeIntervalMet;
    }

    public void SetCameraTarget(CameraTarget Target, bool YieldMovement, System.Action<CameraSystem> Finished)
    {
        PreviousMoveType = Player.GetMoveType();
        SkipAutoTargetSetting = true;
        YieldMovementDuringSet = YieldMovement;

        if (YieldMovement) Player.SetMovementType(MovementType.None, true);

        CurrentTarget = Target;
        FinishedAction = Finished;
    }

    public void SetCameraTarget(Transform Transform, bool YieldMovement, System.Action<CameraSystem> Finished)
    {
        CameraTarget target = new()
        {
            Position = Transform.position,
            Rotation = Transform.rotation
        };

        SetCameraTarget(target, YieldMovement, Finished);
    }

    private CameraTarget GetPlayerCamPositionAndRotation()
    {
        Transform charTransform = Player.Character.gameObject.transform;
        Vector3 newPos = charTransform.position + Offsets.Position;

        Vector3 vecRot = new(main.transform.rotation.x, main.transform.rotation.y, main.transform.rotation.z);
        Quaternion newRot = Quaternion.Euler(vecRot + Offsets.Rotation.eulerAngles);

        return new()
        {
            Position = newPos,
            Rotation = newRot
        };
    }

    private void LerpCameraTransform(Vector3 Position, Quaternion Rotation)
    {
        if (!LerpCamera)
        {
            main.transform.SetPositionAndRotation(Position, Rotation);
        }
        else
        {
            switch (EasingStyle)
            {
                case EasingStyle.Linear:
                    {
                        Position = Vector3.MoveTowards(main.transform.position, Position, Time.fixedDeltaTime * CameraLerpSpeed);
                        Rotation = Quaternion.Slerp(main.transform.rotation, Rotation, Time.fixedDeltaTime * CameraLerpSpeed);
                    }
                    break;
                
                case EasingStyle.Sine:
                    {
                        Position = Vector3.Lerp(main.transform.position, Position, Time.fixedDeltaTime * CameraLerpSpeed);
                        Rotation = Quaternion.Lerp(main.transform.rotation, Rotation, Time.fixedDeltaTime * CameraLerpSpeed);
                    }
                    break;
            }

            main.transform.SetPositionAndRotation(Position, Rotation);
        }

        if (!SkipAutoTargetSetting) return;
        if (main.transform.position != Position || main.transform.rotation != Rotation) return;

        CameraTarget target = new()
        {
            Position = main.transform.position,
            Rotation = main.transform.rotation
        };

        FinishedMovementToTarget(target);
    }

    private void LerpCameraTransform(CameraTarget target) => LerpCameraTransform(target.Position, target.Rotation);

    private void Update()
    {
        if (!MovingBetweenPoints) return;

        if (CurrentTime < MaxTime)
        {
            CurrentTime += Time.deltaTime + ModifierTime;
            return;
        }

        CurrentTime = 0.0f;
        MaxTime = 0.0f;
        ModifierTime = 0.0f;

        TimeIntervalReached?.Invoke();
    }

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

        if (!SkipAutoTargetSetting && TargetPlayer) CurrentTarget = GetPlayerCamPositionAndRotation();

        LerpCameraTransform(CurrentTarget);
    }
    
    private void Awake()
    {
        main = GetComponentInChildren<Camera>();
        DefaultMoveType = Player.GetMoveType();
        DefaultOffsets = Offsets;
    }
}
