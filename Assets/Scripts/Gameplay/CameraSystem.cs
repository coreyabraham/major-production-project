using UnityEngine;
using UnityEngine.Events;

public class CameraSystem : MonoBehaviour
{
    [System.Serializable]
    public class CameraEvents
    {
        public UnityEvent CutsceneStarted;
        public UnityEvent CutsceneFinished;
    }

    [field: Header("Booleans")]
    [field: SerializeField] private bool IgnorePlayerJumps;

    [field: Header("Angles and Offsets")]
    [field: SerializeField] private CameraTarget Offset;
    [field: SerializeField] private float AnticipationOffset = 0.0f;

    [HideInInspector] private CameraTarget PreviousOffset;
    [HideInInspector] private CameraTarget DefaultOffset;

    [field: Header("View Fields")]
    [field: SerializeField] private Vector2 FieldOfViewClamp = new(0, 180);
    [field: SerializeField] private float FieldOfView = 80.0f;
    [field: SerializeField] private float DepthOfField = 0.0f;

    [field: Header("Lerping Speeds")]
    [field: SerializeField] private float CameraLerpSpeed;
    [field: SerializeField] private float VFXLerpSpeed;

    [field: Header("Enumerations")]
    [field: SerializeField] private EasingStyle EasingStyle = EasingStyle.Sine;
    [field: SerializeField] private CameraType CameraType = CameraType.Follow;

    [field: Header("External References")]
    [field: SerializeField] private GameObject CameraSubject;
    public PlayerSystem Player;
    [HideInInspector] public Camera main;

    [field: Space(2.5f)]
    [field: SerializeField] private CameraEvents Events;

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

    public bool IsCutsceneActive() => CutsceneRunning;

    public Transform GetCameraTransform() => main.transform;
    public CameraTarget GetCameraOffset() => Offset;

    public void RevertCameraOffsets() => Offset = PreviousOffset;
    public void SetToDefaultOffsets() => Offset = DefaultOffset;

    public void SetCameraOffsets(Vector3 Position, Quaternion Rotation)
    {
        PreviousOffset = Offset;
        Offset = new()
        {
            position = Position,
            rotation = Rotation
        };
    }
    public void SetCameraOffsets(CameraTarget Target) => SetCameraOffsets(Target.position, Target.rotation);
    public void SetCameraOffsets(Vector3 Target) => SetCameraOffsets(Target, PreviousCameraLocation.rotation);
    public void SetCameraOffsets(Quaternion Target) => SetCameraOffsets(PreviousCameraLocation.position, Target);

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

    private CameraTarget GetCamPositionAndRotation()
    {
        Vector3 newPos = CameraSubject.transform.position + Offset.position;

        // TODO: This needs to be improved!
        if (IgnorePlayerJumps && !Player.IsPlayerGrounded() && !Player.IsClimbing && !Player.IsJumpingFromClimb && !Player.FallingFromClimb)
        {
            newPos.y = (newPos.y >= GroundCameraPosition.y) ? GroundCameraPosition.y : newPos.y;
        }

        if (Player.IsPlayerMoving() && CameraSubject == Player.gameObject)
        {
            Vector2 moveInput = Player.GetMoveInput();
            if (moveInput.x > 0) newPos.x += AnticipationOffset;
            else if (moveInput.x < 0) newPos.x -= AnticipationOffset;
        }

        Vector3 vecRot = new(main.transform.rotation.x, main.transform.rotation.y, main.transform.rotation.z);
        Quaternion newRot = Quaternion.Euler(vecRot + Offset.rotation.eulerAngles);

        CameraTarget target = new()
        {
            position = newPos,
            rotation = newRot
        };

        if (Player.IsPlayerGrounded()) GroundCameraPosition = target.position;

        return target;
    }

    private void LerpCameraTransform(Vector3 Position, Quaternion Rotation, float CustomLerpSpeed = -1.0f)
    {
        float LerpSpeed = (CustomLerpSpeed > 0) ? CustomLerpSpeed : CameraLerpSpeed;

        if (EasingStyle != EasingStyle.None)
        {
            switch (EasingStyle)
            {
                case EasingStyle.Linear:
                    {
                        Position = Vector3.MoveTowards(main.transform.position, Position, Time.fixedDeltaTime * LerpSpeed);
                        Rotation = Quaternion.Slerp(main.transform.rotation, Rotation, Time.fixedDeltaTime * LerpSpeed);
                    }
                    break;
                
                case EasingStyle.Sine:
                    {
                        Position = Vector3.Lerp(main.transform.position, Position, Time.fixedDeltaTime * LerpSpeed);
                        Rotation = Quaternion.Lerp(main.transform.rotation, Rotation, Time.fixedDeltaTime * LerpSpeed);
                    }
                    break;
            }
        }

        main.transform.SetPositionAndRotation(Position, Rotation);

        PreviousCameraLocation = new()
        {
            position = Position,
            rotation = Rotation
        };

        if (!CutsceneRunning || TrackCutsceneInterval) return;
        if (main.transform.position != CutscenePoints[CutsceneIndex].position || main.transform.rotation != CutscenePoints[CutsceneIndex].rotation) return;

        TrackCutsceneInterval = true;
    }

    private void LerpCameraTransform(CameraTarget Target, float CustomLerpSpeed = -1.0f) => LerpCameraTransform(Target.position, Target.rotation, CustomLerpSpeed);

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
        float CameraFOV = Mathf.Clamp(FieldOfView, FieldOfViewClamp.x, FieldOfViewClamp.y);

        if (EasingStyle != EasingStyle.None)
        {
            main.fieldOfView = Mathf.Lerp(main.fieldOfView, CameraFOV, Time.fixedDeltaTime * VFXLerpSpeed);
            //main.depthOfField = Mathf.Lerp(main.depthOfField, DepthOfField, Time.fixedDeltaTime * SettingsLerpSpeed);
        }
        else
        {
            main.fieldOfView = CameraFOV;
            //main.depthOfField = DepthOfField;
        }

        CameraTarget Target = (!CutsceneRunning) ? PreviousCameraLocation : CutscenePoints[CutsceneIndex];

        if (!CutsceneRunning && CameraType != CameraType.Scriptable)
        {
            switch (CameraType)
            {
                case CameraType.Fixed: break;
                case CameraType.Follow: Target = GetCamPositionAndRotation(); break;
                case CameraType.Dolly: Debug.LogWarning(name + " | Couldn't "); break;
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
    }
}
