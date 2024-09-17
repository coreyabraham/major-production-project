using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.HighDefinition;
using static UnityEngine.GraphicsBuffer;

public class CameraSystem : MonoBehaviour
{
    [System.Serializable]
    public class CameraEvents
    {
        public UnityEvent CutsceneStarted;
        public UnityEvent CutsceneFinished;
    }

    [field: Header("Booleans")]
    [field: SerializeField] private bool IgnorePlayerJumps = false;
    public bool IgnoreAnticipationOffset = false;
    public bool SeparateOffsets = false;

    [field: Header("Angles and Offsets")]
    [field: SerializeField] private CameraTarget Offset;
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
    public PlayerSystem Player;
    [HideInInspector] public Camera main;

    [field: Space(2.5f)]
    [field: SerializeField] private CameraEvents Events;

    [HideInInspector] private CameraTarget PreviousOffset;
    [HideInInspector] private CameraTarget DefaultOffset;

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

    private DepthOfField DOF;

    public bool IsCutsceneActive() => CutsceneRunning;

    public CameraType GetCameraType() => CameraType;
    public void SetCameraType(CameraType Enumeration) => CameraType = Enumeration;

    public Transform GetCameraTransform() => main.transform;

    public void SetCameraTransform(Vector3 Position, Quaternion Rotation) => main.transform.SetPositionAndRotation(Position, Rotation);
    public void SetCameraTransform(CameraTarget Transform) => SetCameraTransform(Transform.position, Transform.rotation);
    public void SetCameraTransform(Transform Transform) => SetCameraTransform(Transform.position, Transform.rotation);
    
    public void SetCameraTransform(Vector3 Position) => main.transform.position = Position;
    public void SetCameraTransform(Quaternion Rotation) => main.transform.rotation = Rotation;

    public CameraTarget GetCameraOffset() => Offset;

    public void RevertCameraOffsets() => Offset = PreviousOffset;
    public void SetToDefaultOffsets() => Offset = DefaultOffset;

    public void SetCameraOffset(Vector3 Position, Quaternion Rotation)
    {
        PreviousOffset = Offset;
        Offset = new()
        {
            position = Position,
            rotation = Rotation
        };
    }
    public void SetCameraOffset(CameraTarget Target) => SetCameraOffset(Target.position, Target.rotation);
    public void SetCameraOffset(Transform Target) => SetCameraOffset(Target.position, Target.rotation);
    public void SetCameraOffset(Vector3 Target) => SetCameraOffset(Target, PreviousOffset.rotation);
    public void SetCameraOffset(Quaternion Target) => SetCameraOffset(PreviousOffset.position, Target);

    public void BeginCutscene(CameraTarget[] Points, float TimeInterval, float CameraSpeed = -1.0f)
    {
        if (CutsceneRunning)
        {
            Debug.LogWarning(name + " | A Cutscene is already running! Please wait for it to end or cancel it via `CancelCutscene()`!");
            return;
        }

        PreviousMoveType = Player.GetMoveType();
        Player.SetMoveType(MoveType.None, true);

        CameraType = CameraType.Scriptable;

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

        if (CutsceneIndex >= CutscenePoints.Length)
        {
            CutsceneFinished();
            return;
        }
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

        Player.SetMoveType(PreviousMoveType, true);
        Events.CutsceneFinished?.Invoke();
    }

    private CameraTarget GetPanningTransformation()
    {
        Vector3 targetDirection = CameraSubject.transform.position - PreviousCameraLocation.position;
        Quaternion targetRot = Quaternion.LookRotation(targetDirection, Vector3.up) * Offset.rotation;

        CameraTarget Target = new()
        {
            position = PreviousCameraLocation.position,
            rotation = targetRot
        };

        return Target;
    }

    private CameraTarget GetFollowTransformation()
    {
        Vector3 posModifier = (!SeparateOffsets) ? Offset.position : Vector3.zero;
        Vector3 newPos = CameraSubject.transform.position + posModifier;

        // TODO: This needs to be improved!
        if (IgnorePlayerJumps && !Player.IsPlayerGrounded() && !Player.IsClimbing && !Player.IsJumpingFromClimb && !Player.FallingFromClimb)
        {
            newPos.y = (newPos.y >= GroundCameraPosition.y) ? GroundCameraPosition.y : newPos.y;
        }

        if (Player.IsPlayerMoving() && CameraSubject == Player.gameObject)
        {
            Vector2 moveInput = Player.GetMoveInput();
            if (moveInput.x > 0) newPos.x += !IgnoreAnticipationOffset ? AnticipationOffset : 0.0f;
            else if (moveInput.x < 0) newPos.x -= !IgnoreAnticipationOffset ? AnticipationOffset : 0.0f;
        }

        Vector3 rotReference = new(main.transform.rotation.x, main.transform.rotation.y, main.transform.rotation.z);
        Vector3 rotModifier = (!SeparateOffsets) ? Offset.rotation.eulerAngles : Vector3.zero;

        Quaternion newRot = Quaternion.Euler(rotReference + rotModifier);

        CameraTarget target = new()
        {
            position = newPos,
            rotation = newRot
        };

        if (Player.IsPlayerGrounded()) GroundCameraPosition = target.position;

        return target;
    }

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
            }
        }

        LerpCameraTransform(Target, CutsceneSpeed);
    }
    
    private void Awake()
    {
        main = GetComponentInChildren<Camera>();
        
        DefaultOffset = Offset;
        PreviousCameraType = CameraType;

        if (!CameraSubject) CameraSubject = Player.gameObject;
        if (!DepthOfFieldData.Volume) return;

        bool result = DepthOfFieldData.Volume.profile.TryGet(out DepthOfField component);

        if (!result)
        {
            Debug.LogWarning(name + " | Couldn't get Depth of Field Processing Effect!");
            return;
        }

        DOF = component;
    }
}
