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

    [field: Header("Toggleables")]
    public bool TargetPlayer = false;

    [field: Header("Angles and Offsets")]
    [field: SerializeField] private CameraTarget Offset;

    [field: SerializeField]

    [HideInInspector] private CameraTarget PreviousOffset;
    [HideInInspector] private CameraTarget DefaultOffset;

    [field: Space(5.0f)]

    [field: SerializeField] private Vector2 FieldOfViewClamp = new(0, 180);
    [field: SerializeField] private float FieldOfView = 80.0f;
    [field: SerializeField] private float DepthOfField;

    [field: Header("Lerping")]
    [field: SerializeField] private bool LerpCamera;
    [field: SerializeField] private bool LerpVFX;

    [field: Space(5.0f)]

    [field: SerializeField] private float CameraLerpSpeed;
    [field: SerializeField] private float VFXLerpSpeed;

    [field: Space(5.0f)]
    
    [field: SerializeField] private EasingStyle EasingStyle;
    [field: SerializeField, Tooltip("Not Implemented Yet!")] private EasingDirection EasingDirection;

    [field: Header("External References")]
    public PlayerSystem Player;
    [HideInInspector] public Camera main;

    [field: Space(2.5f)]
    [field: SerializeField] private CameraEvents Events;

    private bool CutsceneRunning = false;
    private bool TrackCutsceneInterval = false;

    private float CurrentInterval = 0.0f;
    private float MaxInterval = 0.0f;
    private float CutsceneSpeed = 0.0f;

    private int CutsceneIndex = 0;
    private CameraTarget[] CutscenePoints;

    private MovementType PreviousMoveType;

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
    public void SetCameraOffsets(Transform Target) => SetCameraOffsets(Target.position, Target.rotation);

    public void BeginCutscene(CameraTarget[] Points, float TimeInterval, float CameraSpeed = -1.0f)
    {
        if (CutsceneRunning)
        {
            Debug.LogWarning(name + " | A Cutscene is already running! Please wait for it to end or cancel it via `CancelCutscene()`!");
            return;
        }

        /* 
         * TODO:
         * 1. Detach Camera from Player IF the Camera isn't already being used in a Cutscene 
         * 2. Iterate through all Camera Points and time them through `Update()`
         * 3. Start doing a count-up timer to the `Interval` float
         * 4. Once reached, reset the timer then proceed to the next item in the array
         * 5. If there's nothing left, discontinue the loop and run one final interval at the end
         * 6. After the last interval (using `float Duration`), enable `TargetPlayer` once again and continue as normal!
         * 7. Look into potentially adding in Events for when an index is registered, when it finishes and when the entire method finishes!
         */

        PreviousMoveType = Player.GetMovementType();
        Player.SetMovementType(MovementType.None, true);

        TargetPlayer = false;

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
        TargetPlayer = true;

        CurrentInterval = 0.0f;
        MaxInterval = 0.0f;
        CutsceneSpeed = 0.0f;

        CutsceneIndex = 0;
        CutscenePoints = null;

        Player.SetMovementType(PreviousMoveType, true);
        Events.CutsceneFinished?.Invoke();
    }

    private CameraTarget GetPlayerCamPositionAndRotation()
    {
        Transform charTransform = Player.Character.gameObject.transform;
        Vector3 newPos = charTransform.position + Offset.position;

        Vector3 vecRot = new(main.transform.rotation.x, main.transform.rotation.y, main.transform.rotation.z);
        Quaternion newRot = Quaternion.Euler(vecRot + Offset.rotation.eulerAngles);

        CameraTarget target = new()
        {
            position = newPos,
            rotation = newRot
        };

        return target;
    }

    private void LerpCameraTransform(Vector3 Position, Quaternion Rotation, float CustomLerpSpeed = -1.0f)
    {
        float LerpSpeed = (CustomLerpSpeed > 0) ? CustomLerpSpeed : CameraLerpSpeed;

        if (LerpCamera)
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

        if (LerpVFX)
        {
            main.fieldOfView = Mathf.Lerp(main.fieldOfView, CameraFOV, Time.fixedDeltaTime * VFXLerpSpeed);
            //main.depthOfField = Mathf.Lerp(main.depthOfField, DepthOfField, Time.fixedDeltaTime * SettingsLerpSpeed);
        }
        else
        {
            main.fieldOfView = CameraFOV;
            //main.depthOfField = DepthOfField;
        }

        var Target = (!CutsceneRunning) ? GetPlayerCamPositionAndRotation() : CutscenePoints[CutsceneIndex];
        LerpCameraTransform(Target, CutsceneSpeed);
    }
    
    private void Awake()
    {
        main = GetComponentInChildren<Camera>();
        DefaultOffset = Offset;
    }
}
