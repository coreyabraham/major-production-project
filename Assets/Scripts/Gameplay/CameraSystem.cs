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
    [field: SerializeField] private bool IgnorePlayerJumps = false;
    [field: SerializeField] private bool AutomateStartCameraOffset = false;
    public bool IgnoreAnticipationOffset = false;
    public bool IgnoreCurrentOffset = false;

    [field: Header("Angles and Offsets")]
    [field: SerializeField] private CameraTarget CurrentOffset;
    [field: SerializeField] private float AnticipationOffset = 0.0f;

    [field: Header("View Fields")]
    [field: SerializeField] private Vector2 FieldOfViewClamp = new(0, 180);
    [field: SerializeField] private float FieldOfView = 80.0f;

    [field: Header("Lerping Speeds")]
    [field: SerializeField] private float CameraLerpSpeed;
    [field: SerializeField] private float VFXLerpSpeed;

    [field: Header("Enumerations")]
    [field: SerializeField] private EasingStyle EasingStyle = EasingStyle.Lerp;
    [field: SerializeField] private CameraType CameraType = CameraType.Follow;

    [field: Header("Depth of Field")]
    [field: SerializeField] private DOF_Data DepthOfFieldData;

    [field: Header("External References")]
    [field: SerializeField] private GameObject CameraSubject;
    //public PlayerSystem Player;
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

    private bool CutsceneRunning = false;
    private bool TrackCutsceneInterval = false;

    private float CurrentInterval = 0.0f;
    private float MaxInterval = 0.0f;
    private float CutsceneSpeed = 0.0f;

    private int CutsceneIndex = 0;
    private CameraTarget[] CutscenePoints;

    private MoveType PreviousMoveType;
    private CameraType PreviousCameraType;

    private float DefaultFOV;

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
        Vector3 posModifier = (!IgnoreCurrentOffset) ? CurrentOffset.position : Vector3.zero;

        Vector3 tarPos = CameraSubject != null ? CameraSubject.transform.position : PreviousCameraLocation.position;
        Vector3 newPos = tarPos + posModifier;

        Vector3 rotReference = new(main.transform.rotation.x, main.transform.rotation.y, main.transform.rotation.z);
        Vector3 rotModifier = (!IgnoreCurrentOffset) ? CurrentOffset.rotation.eulerAngles : Vector3.zero;
        Quaternion newRot = Quaternion.Euler(rotReference + rotModifier);

        return new()
        {
            position = newPos,
            rotation = newRot
        };
    }

    public void SetCameraType(CameraType Enumeration) => CameraType = Enumeration;
    public void RevertCameraType() => CameraType = PreviousCameraType;

    public void LerpCameraTransform(Vector3 Position, Quaternion Rotation, float CustomLerpSpeed = float.MinValue)
    {
        float LerpValue = CustomLerpSpeed > 0.0f ? CustomLerpSpeed : Time.fixedDeltaTime * CameraLerpSpeed;

        if (EasingStyle != EasingStyle.None)
        {
            switch (EasingStyle)
            {
                case EasingStyle.Basic:
                    {
                        Position = Vector3.MoveTowards(main.transform.position, Position, LerpValue);
                        Rotation = Quaternion.RotateTowards(main.transform.rotation, Rotation, LerpValue);
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
            rotation = Zone.TargetRotation
        };

        FieldOfView = Zone.TargetFOV;

        if (Zone.OverridePreviousPosition) { PreviousOffset.position = Zone.TargetPosition; }
        if (Zone.OverridePreviousRotation) { PreviousOffset.rotation = Zone.TargetRotation; }
    }

    public void ResetZoneCamOffset()
    {
        CurrentOffset = PreviousOffset;
        FieldOfView = DefaultFOV;
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
        (PreviousCameraType, CameraType) = (CameraType, PreviousCameraType);
        
        CurrentInterval = 0.0f;
        MaxInterval = 0.0f;
        CutsceneSpeed = 0.0f;

        CutsceneIndex = 0;
        CutscenePoints = null;

        CurrentState = CameraState.Generic;

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

    private CameraTarget GetPanningTransformation()
    {
        Vector3 tarPos = CameraSubject != null ? CameraSubject.transform.position : PreviousCameraLocation.position;

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

        // TODO: This needs to be improved!
        if (IgnorePlayerJumps && !ActivePlayer.IsPlayerGrounded() && !ActivePlayer.IsClimbing && !ActivePlayer.IsJumpingFromClimb && !ActivePlayer.FallingFromClimb)
        {
            cameraDelta.position.y = (cameraDelta.position.y >= GroundCameraPosition.y) ? GroundCameraPosition.y : cameraDelta.position.y;
        }

        if (ActivePlayer)
        {
            if (ActivePlayer.IsPlayerMoving() && CameraSubject == ActivePlayer.gameObject)
            {
                cameraDelta.position.x += GetAnticipationOffset();
            }

            if (ActivePlayer.IsPlayerGrounded()) GroundCameraPosition = cameraDelta.position;
        }

        return cameraDelta;
    }

    private float GetOffsetAndTargetEvaluation()
    {
        // Get distance between triggerzone and player

        Vector3 tarPos = CameraSubject != null ? CameraSubject.transform.position : PreviousCameraLocation.position;

        // (Might have to replace `ActiveZoneTrigger.transform.position.x` with respective LocalScale#!)
        float distanceFromTriggerCentre = Mathf.Abs(ActiveZoneTrigger.transform.position.x - tarPos.x);

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

        Vector3 newCamOffset = ActiveZoneTrigger.UsePositionOffset ? Vector3.Lerp(CurrentOffset.position, ActiveZoneTrigger.TargetPosition, curveEvaluation) : CurrentOffset.position;

        Vector3 targetCamPos = new Vector3(
            CameraSubject.transform.position.x + newCamOffset.x + GetAnticipationOffset(),
            CameraSubject.transform.position.y + newCamOffset.y,
            CameraSubject.transform.position.z + newCamOffset.z
        );

        Quaternion quaternion;

        if (ActiveZoneTrigger.UseRotationOffset)
        {
            //HANDLES BLENDED ROTATION
            Vector3 targetCamRotation = Vector3.Lerp(CurrentOffset.rotation.eulerAngles, ActiveZoneTrigger.TargetRotation.eulerAngles, curveEvaluation);
            quaternion = Quaternion.Euler(targetCamRotation.x, targetCamRotation.y, targetCamRotation.z);
        }
        else
        {
            //HANDLES ROTATION
            quaternion = Quaternion.Euler(CurrentOffset.rotation.x, CurrentOffset.rotation.y, CurrentOffset.rotation.z);
        }

        FieldOfView = ActiveZoneTrigger.UseFOVAdjustment ? Mathf.Lerp(FieldOfView, ActiveZoneTrigger.TargetFOV, curveEvaluation) : FieldOfView;

        // TODO: INTERGRATE `GetFollowTransformation()` INTO THIS RESULT!
        return new()
        {
            position = targetCamPos,
            rotation = quaternion
        };
    }

    // TODO: IMPLEMENT!
    private CameraTarget GetTargetTransformation()
    {
        if (!ActiveZoneTrigger || !CameraSubject) return GetFollowTransformation();

        float curveEvaluation = GetOffsetAndTargetEvaluation();

        Vector3 targetCamPos = new Vector3(
            CameraSubject.transform.position.x + CurrentOffset.position.x + GetAnticipationOffset(),
            CameraSubject.transform.position.y + CurrentOffset.position.y,
            CameraSubject.transform.position.z + CurrentOffset.position.z
        );

        Quaternion quaternion;

        if (ActiveZoneTrigger.UseRotationOffset)
        {
            //HANDLES BLENDED ROTATION
            Vector3 targetCamRotation = Vector3.Lerp(CurrentOffset.rotation.eulerAngles, ActiveZoneTrigger.TargetRotation.eulerAngles, curveEvaluation);
            quaternion = Quaternion.Euler(targetCamRotation.x, targetCamRotation.y, targetCamRotation.z);
        }
        else
        {
            //HANDLES ROTATION
            quaternion = Quaternion.Euler(CurrentOffset.rotation.x, CurrentOffset.rotation.y, CurrentOffset.rotation.z);
        }

        FieldOfView = ActiveZoneTrigger.UseFOVAdjustment ? Mathf.Lerp(FieldOfView, ActiveZoneTrigger.TargetFOV, curveEvaluation) : FieldOfView;

        return new()
        {
            position = targetCamPos,
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

        // TODO: THIS OR SPLIT THIS ENTIRE BIT THAT IS ESSENTIALLY BLOAT!
        #region TEMPORARY_REGION
        float CameraFOV = Mathf.Clamp(FieldOfView, FieldOfViewClamp.x, FieldOfViewClamp.y);

        if (DOF) DOF.active = DepthOfFieldData.Active;

        if (EasingStyle != EasingStyle.None)
        {
            main.fieldOfView = Mathf.Lerp(main.fieldOfView, CameraFOV, Time.fixedDeltaTime * VFXLerpSpeed);

            if (DOF != null && DOF.active)
            {
                DOF.nearFocusStart.Interp(DOF.nearFocusStart.value, DepthOfFieldData.NearRangeStart.value, Time.fixedDeltaTime * VFXLerpSpeed);
                DOF.nearFocusEnd.Interp(DOF.nearFocusEnd.value, DepthOfFieldData.NearRangeEnd.value, Time.fixedDeltaTime * VFXLerpSpeed);

                DOF.farFocusStart.Interp(DOF.farFocusStart.value, DepthOfFieldData.FarRangeStart.value, Time.fixedDeltaTime * VFXLerpSpeed);
                DOF.farFocusEnd.Interp(DOF.farFocusEnd.value, DepthOfFieldData.FarRangeEnd.value, Time.fixedDeltaTime * VFXLerpSpeed);

                DOF.nearSampleCount = (int)Mathf.Lerp(DOF.nearSampleCount, DepthOfFieldData.NearSampleCount, Time.fixedDeltaTime * VFXLerpSpeed);
                DOF.farSampleCount = (int)Mathf.Lerp(DOF.farSampleCount, DepthOfFieldData.FarSampleCount, Time.fixedDeltaTime * VFXLerpSpeed);

                DOF.nearMaxBlur = Mathf.Lerp(DOF.nearMaxBlur, DepthOfFieldData.NearMaxBlur, Time.fixedDeltaTime * VFXLerpSpeed);
                DOF.farMaxBlur = Mathf.Lerp(DOF.farMaxBlur, DepthOfFieldData.FarMaxBlur, Time.fixedDeltaTime * VFXLerpSpeed);
            }
        }
        else
        {
            main.fieldOfView = CameraFOV;

            if (DOF != null && DOF.active)
            {
                DOF.nearFocusStart = DepthOfFieldData.NearRangeStart;
                DOF.nearFocusEnd = DepthOfFieldData.NearRangeEnd;

                DOF.farFocusStart = DepthOfFieldData.FarRangeStart;
                DOF.farFocusEnd = DepthOfFieldData.FarRangeEnd;

                DOF.nearSampleCount = DepthOfFieldData.NearSampleCount;
                DOF.farSampleCount = DepthOfFieldData.FarSampleCount;

                DOF.nearMaxBlur = DepthOfFieldData.NearMaxBlur;
                DOF.farMaxBlur = DepthOfFieldData.FarMaxBlur;
            }
        }

        if (DOF != null && DOF.active)
        {
            DOF.nearFocusStart.overrideState = DepthOfFieldData.NearRangeStart.overrideState;
            DOF.nearFocusEnd.overrideState = DepthOfFieldData.NearRangeEnd.overrideState;

            DOF.farFocusStart.overrideState = DepthOfFieldData.FarRangeStart.overrideState;
            DOF.farFocusEnd.overrideState = DepthOfFieldData.FarRangeEnd.overrideState;

            DOF.focusMode = DepthOfFieldData.FocusMode;

            DOF.quality.levelAndOverride = ((int)DepthOfFieldData.Quality.quality, DepthOfFieldData.Quality.ignoreOverride);
        }
        #endregion

        CameraTarget Target = CutsceneRunning ? CutscenePoints[CutsceneIndex] : PreviousCameraLocation;

        if (!CutsceneRunning && CameraType != CameraType.Scriptable)
        {
            switch (CameraType)
            {
                case CameraType.Follow: Target = GetFollowTransformation(); break;
                case CameraType.Tracking: Target = GetPanningTransformation(); break;

                case CameraType.OffsetState: Target = GetOffsetTransformation(); break;
                case CameraType.TargetState: Target = GetTargetTransformation(); break;
            }
        }

        LerpCameraTransform(Target, CutsceneSpeed);
    }

    private void Start()
    {
        PreviousCameraSubject = CameraSubject;

        if (!CameraSubject) CameraSubject = GameSystem.Instance.Player.gameObject;
        if (AutomateStartCameraOffset) CurrentOffset.position = transform.position - CameraSubject.transform.position;

        DefaultOffset = CurrentOffset;
        PreviousCameraType = CameraType;

        DefaultFOV = FieldOfView;

        //DefaultRotation = main.transform.localRotation;
        //CurrentRotation = DefaultRotation;

        //DefaultPosOffset = CurrentPosOffset;

        if (!DepthOfFieldData.Volume) return;

        bool result = DepthOfFieldData.Volume.profile.TryGet(out DepthOfField component);

        if (!result)
        {
            Debug.LogWarning(name + " | Couldn't get Depth of Field Processing Effect!");
            return;
        }

        DOF = component;
    }

    private void Awake() => main = GetComponentInChildren<Camera>();
}
