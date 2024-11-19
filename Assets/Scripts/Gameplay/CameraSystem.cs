using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.HighDefinition;

public class CameraSystem : MonoBehaviour
{
    [System.Serializable]
    public class CameraEvents
    {
        public UnityEvent CutsceneStarted;
        public UnityEvent CutsceneFinished;
    }

    #region Public / Serialized Private Variables
    [field: Header("Booleans")]
    [field: SerializeField] private bool CameraJumpsWithPlayer = false;
    [field: SerializeField] private bool AutomateStartCameraOffset = false;
    public bool IgnoreAnticipationOffset = false;
    public bool IgnoreCurrentOffset = false;

    [field: Header("Angles and Offsets")]
    [field: SerializeField] private CameraTarget CurrentOffset;
    [field: SerializeField] private float AnticipationOffset = 0.0f;
    public float FieldOfView = 80.0f;

    [field: Header("Lerping Speeds")]
    [field: SerializeField] private float CameraLerpSpeed;
    [field: SerializeField] private float VFXLerpSpeed;
    [field: SerializeField] private float AnticipationSpeed;

    [field: Header("Enumerations")]
    [field: SerializeField] private EasingStyle EasingStyle = EasingStyle.Lerp;
    [field: SerializeField] private CameraType CameraType = CameraType.Follow;

    [field: Header("External References")]
    [field: SerializeField] private GameObject CameraSubject;
    [HideInInspector] public Camera main;

    [field: Space(2.5f)]
    [field: SerializeField] private CameraEvents Events;
    #endregion

    #region Private Variables
    private CameraState CurrentState;

    private CameraTarget PreviousOffset;
    private CameraTarget DefaultOffset;

    private CameraTarget PreviousCameraLocation;
    private Vector3 GroundCameraPosition;

    private bool IgnoreCameraJumping = false;

    private bool CutsceneRunning = false;
    private bool TrackCutsceneInterval = false;

    private float CurrentInterval = 0.0f;
    private float MaxInterval = 0.0f;
    private float CutsceneSpeed = -1.0f;

    private int CutsceneIndex = 0;
    private CameraTarget[] CutscenePoints;

    private MoveType PreviousMoveType;
    private CameraType PreviousCameraType;

    private float PreviousFOV;

    private ZoneCamera ActiveZoneTrigger;

    private PlayerSystem ActivePlayer = null;
    private GameObject PreviousCameraSubject = null;

    private DepthOfField DOF;
    #endregion

    #region Getter and Setter Methods
    public CameraState GetCameraState() => CurrentState;

    public void OverrideCameraOffset(CameraTarget Offset) => CurrentOffset = Offset;
    public void OverrideCameraOffset(Vector3 Position, Quaternion Rotation)
    {
        CameraTarget target = new()
        {
            position = Position,
            rotation = Rotation
        };

        OverrideCameraOffset(target);
    }
    public void OverrideCameraOffset(Vector3 Position)
    {
        CameraTarget target = new()
        {
            position = Position
        };

        OverrideCameraOffset(target);
    }
    public void OverrideCameraOffset(Quaternion Rotation)
    {
        CameraTarget target = new()
        {
            rotation = Rotation
        };

        OverrideCameraOffset(target);
    }

    public CameraTarget GetCameraOffset() => CurrentOffset;
    public CameraTarget ResetCameraOffset() => CurrentOffset = DefaultOffset;

    public CameraTarget GetCameraDelta()
    {
        Vector3 tarPos = CameraSubject != null ? CameraSubject.transform.position : GroundCameraPosition;
        Vector3 posModifier = !IgnoreCurrentOffset ? CurrentOffset.position : Vector3.zero;
        
        Vector3 position = tarPos + posModifier;
        Quaternion rotation = !IgnoreCurrentOffset ? CurrentOffset.rotation : new Quaternion();

        return new()
        {
            position = position,
            rotation = rotation
        };
    }

    public void SetCameraType(CameraType Enumeration) => CameraType = Enumeration;
    public void RevertCameraType() => CameraType = PreviousCameraType;

    public void ForceCameraTarget(CameraTarget Target) => main.transform.SetPositionAndRotation(Target.position, Target.rotation);
    public void ForceCameraTarget()
    {
        CameraTarget target = GetFollowTransformation();

        switch (CameraType)
        {
            case CameraType.Tracking: target = GetTrackingTransformation(); break;
            case CameraType.OffsetState: target = GetOffsetTransformation(); break;
            case CameraType.TargetState: target = GetTargetTransformation(); break;
        }

        ForceCameraTarget(target);
    }

    public void LerpCameraTransform(Vector3 Position, Quaternion Rotation, float CustomLerpSpeed = float.MinValue)
    {
        float DefaultLerpSpeed = CameraLerpSpeed * Time.fixedDeltaTime;
        float LerpValue = CustomLerpSpeed > 0.0f ? CustomLerpSpeed : DefaultLerpSpeed;

        Vector3 EularRotation = Rotation.eulerAngles;

        if (EasingStyle != EasingStyle.None)
        {
            switch (EasingStyle)
            {
                case EasingStyle.Basic:
                    {
                        Position = Vector3.MoveTowards(main.transform.position, Position, LerpValue);
                        EularRotation = Vector3.MoveTowards(main.transform.rotation.eulerAngles, EularRotation, LerpValue);
                    }
                    break;

                case EasingStyle.Lerp:
                    {
                        Position = Vector3.Lerp(main.transform.position, Position, LerpValue);
                        Rotation = Quaternion.Lerp(main.transform.rotation, Rotation, LerpValue);
                    }
                    break;

                case EasingStyle.Slerp:
                    {
                        Position = Vector3.Slerp(main.transform.position, Position, LerpValue);
                        Rotation = Quaternion.Slerp(main.transform.rotation, Rotation, LerpValue);
                    }
                    break;
            }
        }

        PreviousCameraLocation = new()
        {
            position = main.transform.position,
            rotation = main.transform.rotation
        };

        main.transform.SetPositionAndRotation(Position, Rotation);

        if (!CutsceneRunning || TrackCutsceneInterval) return;
        if (main.transform.position != CutscenePoints[CutsceneIndex].position || main.transform.rotation != CutscenePoints[CutsceneIndex].rotation) return;

        TrackCutsceneInterval = true;
    }

    public void LerpCameraTransform(CameraTarget Target, float CustomLerpSpeed = -1.0f) => LerpCameraTransform(Target.position, Target.rotation, CustomLerpSpeed);
    public void LerpCameraTransform(Vector3 Position, float CustomLerpSpeed = -1.0f) => LerpCameraTransform(Position, PreviousCameraLocation.rotation, CustomLerpSpeed);
    public void LerpCameraTransform(Quaternion Rotation, float CustomLerpSpeed = -1.0f) => LerpCameraTransform(PreviousCameraLocation.position, Rotation, CustomLerpSpeed);

    public void SetActiveTriggerZone(ZoneCamera Zone) => ActiveZoneTrigger = Zone;

    public void GetNewZoneCameraOffsets(ZoneCamera Zone)
    {
        CurrentOffset = new()
        {
            position = Zone.TargetPosition,
            rotation = Quaternion.Euler(Zone.TargetRotation)
        };

        PreviousOffset.position = Zone.OverridePreviousPosition ? Zone.TargetPosition : PreviousOffset.position;
        PreviousOffset.rotation = Zone.OverridePreviousRotation ? Quaternion.Euler(Zone.TargetRotation) : PreviousOffset.rotation;
        PreviousFOV = Zone.OverridePreviousFOV ? Zone.TargetFOV : PreviousFOV;
    }

    public void ResetZoneCamOffset()
    {
        CurrentOffset = PreviousOffset;
        FieldOfView = PreviousFOV;
    }
    #endregion

    #region Cutscene Methods
    public void BeginCutscene(CameraTarget[] Points, float TimeInterval, float CameraSpeed = -1.0f)
    {
        if (CutsceneRunning)
        {
            Debug.LogWarning(name + " | A Cutscene is already running! Please wait for it to end or cancel it via `CancelCutscene()`!");
            return;
        }

        if (ActivePlayer)
        {
            PreviousMoveType = ActivePlayer.GetMoveType();
            ActivePlayer.SetMoveType(MoveType.None, true);
        }

        CameraType = CameraType.Scriptable;
        CurrentState = CameraState.Cutscene;

        CurrentInterval = 0.0f;
        MaxInterval = TimeInterval;
        CutsceneSpeed = CameraSpeed;

        CutscenePoints = Points;
        CutsceneRunning = true;

        Events.CutsceneStarted?.Invoke();
    }

    public void BeginCutscene(GameObject[] Points, float TimeInterval, float CameraSpeed = -1.0f)
    {
        CameraTarget[] POIs = new CameraTarget[Points.Length];

        for (int i = 0; i < Points.Length; i++)
        {
            POIs[i] = new()
            {
                position = Points[i].transform.position,
                rotation = Points[i].transform.rotation,
            };
        }

        BeginCutscene(POIs, TimeInterval, CameraSpeed);
    }

    public void BeginCutscene(Transform[] Points, float TimeInterval, float CameraSpeed = -1.0f)
    {
        CameraTarget[] POIs = new CameraTarget[Points.Length];

        for (int i = 0; i < Points.Length; i++)
        {
            POIs[i] = new()
            {
                position = Points[i].position,
                rotation = Points[i].rotation,
            };
        }

        BeginCutscene(POIs, TimeInterval, CameraSpeed);
    }

    private void UpdateCutsceneIndex()
    {
        CutsceneIndex++;
        if (CutsceneIndex < CutscenePoints.Length) return;
        CutsceneFinished();
    }

    private void CutsceneFinished()
    {
        CutsceneRunning = false;
        
        CurrentInterval = 0.0f;
        MaxInterval = 0.0f;
        CutsceneSpeed = -1.0f;

        CutsceneIndex = 0;
        CutscenePoints = null;

        CurrentState = CameraState.Generic;
        (PreviousCameraType, CameraType) = (CameraType, PreviousCameraType);

        if (ActivePlayer) ActivePlayer.SetMoveType(PreviousMoveType, true);
        
        Events.CutsceneFinished?.Invoke();
    }
    #endregion

    #region Get Camera Transformations
    private float GetAnticipationOffset()
    {
        if (!ActivePlayer) return 0.0f;

        float anticipation = 0.0f;
        Vector2 moveInput = ActivePlayer.GetMoveInput();

        if (moveInput.x > 0) anticipation += !IgnoreAnticipationOffset ? AnticipationOffset : 0.0f;
        else if (moveInput.x < 0) anticipation -= !IgnoreAnticipationOffset ? AnticipationOffset : 0.0f;

        return anticipation;
    }

    private CameraTarget GetTrackingTransformation()
    {
        Vector3 tarPos = CameraSubject != null ? CameraSubject.transform.position : GroundCameraPosition;

        Vector3 targetDirection = tarPos - PreviousCameraLocation.position;
        Quaternion targetRot = Quaternion.LookRotation(targetDirection, Vector3.up) * CurrentOffset.rotation;

        CameraTarget Target = new()
        {
            position = PreviousCameraLocation.position,
            rotation = targetRot
        };

        return Target;
    }

    private CameraTarget GetFollowTransformation()
    {
        CameraTarget cameraDelta = GetCameraDelta();
        if (!ActivePlayer) return cameraDelta;  

        if (IgnoreCameraJumping) cameraDelta.position.y = cameraDelta.position.y >= GroundCameraPosition.y ? GroundCameraPosition.y : cameraDelta.position.y;

        if (ActivePlayer)
        {
            if (ActivePlayer.IsPlayerMoving() && CameraSubject == ActivePlayer.gameObject)
            {
                cameraDelta.position.x = Mathf.Lerp(cameraDelta.position.x, cameraDelta.position.x + GetAnticipationOffset(), Time.fixedDeltaTime * AnticipationSpeed);
            }

            if (ActivePlayer.IsPlayerGrounded()) GroundCameraPosition = cameraDelta.position;
        }

        return cameraDelta;
    }

    private float GetOffsetAndTargetEvaluation()
    {
        // Get distance between triggerzone and player
        Vector3 tarPos = CameraSubject != null ? CameraSubject.transform.position : GroundCameraPosition;
        float selection = 0.0f;

        switch (ActiveZoneTrigger.LocalScaleType)
        {
            case CartesianCoords.X: selection = ActiveZoneTrigger.transform.position.x - tarPos.x; break;
            case CartesianCoords.Y: selection = ActiveZoneTrigger.transform.position.y - tarPos.y; break;
            case CartesianCoords.Z: selection = ActiveZoneTrigger.transform.position.z - tarPos.z; break;
        }

        float distanceFromTriggerCentre = Mathf.Abs(selection);

        // Get blend amount
        float blendAmount = 1 - distanceFromTriggerCentre / ActiveZoneTrigger.TriggerSize;
        float curveEvaluation = ActiveZoneTrigger.LerpCurve.Evaluate(blendAmount);

        // Return blend amount
        return curveEvaluation;
    }

    private CameraTarget GetOffsetTransformation()
    {
        if (!ActiveZoneTrigger || !CameraSubject) return GetFollowTransformation();

        float curveEvaluation = GetOffsetAndTargetEvaluation();

        Vector3 newCamOffset = ActiveZoneTrigger.UsePositionOffset ? Vector3.Lerp(
            CurrentOffset.position, 
            ActiveZoneTrigger.TargetPosition, 
            curveEvaluation
            ) : CurrentOffset.position;

        Vector3 position = CameraSubject.transform.position + newCamOffset;
        position.x = Mathf.Lerp(position.x, position.x + GetAnticipationOffset(), Time.fixedDeltaTime * AnticipationSpeed);

        if (IgnoreCameraJumping) position.y = PreviousCameraLocation.position.y;

        Vector3 rotation = ActiveZoneTrigger.UseRotationOffset ?
            Vector3.Lerp(
                CurrentOffset.rotation.eulerAngles,
                ActiveZoneTrigger.TargetRotation,
                curveEvaluation
                ) : CurrentOffset.rotation.eulerAngles;

        Quaternion quaternion = Quaternion.Euler(rotation);

        if (ActiveZoneTrigger.UseFOVAdjustment) main.fieldOfView = Mathf.Lerp(FieldOfView, ActiveZoneTrigger.TargetFOV, curveEvaluation);

        return new()
        {
            position = position,
            rotation = quaternion
        };
    }

    private CameraTarget GetTargetTransformation()
    {
        if (!ActiveZoneTrigger || !CameraSubject) return GetFollowTransformation();

        float curveEvaluation = GetOffsetAndTargetEvaluation();
        CameraTarget cameraDelta = GetCameraDelta();

        Vector3 position = ActiveZoneTrigger.UsePositionOffset ? 
            Vector3.Lerp(
                cameraDelta.position,
                ActiveZoneTrigger.TargetPosition,
                curveEvaluation
                ) : ActiveZoneTrigger.TargetPosition;

        if (IgnoreCameraJumping) position.y = PreviousCameraLocation.position.y;

        Vector3 rotation = ActiveZoneTrigger.UseRotationOffset ?
            Vector3.Lerp(
                    cameraDelta.rotation.eulerAngles,
                    ActiveZoneTrigger.TargetRotation,
                    curveEvaluation
                ) : ActiveZoneTrigger.TargetRotation;

        Quaternion quaternion = Quaternion.Euler(rotation);

        if (ActiveZoneTrigger.UseFOVAdjustment) main.fieldOfView = Mathf.Lerp(FieldOfView, ActiveZoneTrigger.TargetFOV, curveEvaluation);

        return new()
        {
            position = position,
            rotation = quaternion
        };
    }
    #endregion

    private void Update()
    {
        if (!CutsceneRunning || !TrackCutsceneInterval) return;

        if (CurrentInterval < MaxInterval)
        {
            CurrentInterval += Time.deltaTime;
            return;
        }

        CurrentInterval = 0.0f;
        TrackCutsceneInterval = false;

        UpdateCutsceneIndex();
    }

    private void FixedUpdate()
    {
        if (PreviousCameraSubject != CameraSubject)
        {
            bool result = false;
            if (CameraSubject != null) result = CameraSubject.TryGetComponent(out ActivePlayer);

            if (!result) ActivePlayer = null;
            PreviousCameraSubject = CameraSubject;
        }

        if (CameraType != CameraType.TargetState || CameraType != CameraType.OffsetState)
        {
            main.fieldOfView = EasingStyle != EasingStyle.None 
                ? Mathf.Lerp(
                    main.fieldOfView, 
                    FieldOfView, 
                    Time.fixedDeltaTime * VFXLerpSpeed) : FieldOfView;
        }

        if (ActivePlayer != null)
        {
            IgnoreCameraJumping = CameraJumpsWithPlayer
                && !ActivePlayer.IsPlayerGrounded()
                && !ActivePlayer.IsClimbing
                && !ActivePlayer.IsJumpingFromClimb
                && !ActivePlayer.FallingFromClimb;
        }

        CameraTarget Target = !CutsceneRunning ? PreviousCameraLocation : CutscenePoints[CutsceneIndex];

        if (!CutsceneRunning && CameraType != CameraType.Scriptable)
        {
            switch (CameraType)
            {
                case CameraType.Follow: Target = GetFollowTransformation(); break;
                case CameraType.Tracking: Target = GetTrackingTransformation(); break;

                case CameraType.OffsetState: Target = GetOffsetTransformation(); break;
                case CameraType.TargetState: Target = GetTargetTransformation(); break;
            }
        }

        LerpCameraTransform(Target, CutsceneSpeed);
    }

    private void Start()
    {
        if (!CameraSubject) CameraSubject = GameSystem.Instance.Player.gameObject;
        if (AutomateStartCameraOffset) CurrentOffset.position = transform.position - CameraSubject.transform.position;

        DefaultOffset = CurrentOffset;
        PreviousOffset = DefaultOffset;

        PreviousFOV = FieldOfView;

        PreviousCameraType = CameraType;
    }

    private void Awake() => main = GetComponentInChildren<Camera>();
}
