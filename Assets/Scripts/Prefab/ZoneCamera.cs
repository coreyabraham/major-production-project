using System.Linq;
using UnityEngine;

public class ZoneCamera : MonoBehaviour
{
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

    private CameraTarget DefaultOffset;

    private CameraSystem Camera;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        PlayerIsInTrigger = true;

        PreviousSeparateOffsets = Camera.SeparateOffsets;
        PreviousPreviousAnticipationOffset = Camera.IgnoreAnticipationOffset;

        Camera.SeparateOffsets = TargetOffsetObject != null;
        Camera.IgnoreAnticipationOffset = true;

        DefaultOffset = Camera.GetCameraOffset();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        PlayerIsInTrigger = false;

        Camera.SeparateOffsets = PreviousSeparateOffsets;
        Camera.IgnoreAnticipationOffset = PreviousPreviousAnticipationOffset;

        Camera.SetCameraOffset(
            !OverridePreviousPosition ? DefaultOffset.position : TargetPosition,
            !OverridePreviousRotation ? DefaultOffset.rotation : TargetRotation
        );
    }

    private void FixedUpdate()
    {
        if (!PlayerIsInTrigger) return;

        switch (BlendType)
        {
            case ZoneBlendType.OffsetState:
                {
                    if (!ZoneBlending)
                    {
                        if (!TargetOffsetObject)
                        {
                            if (UsePositionOffset && !UseRotationOffset) Camera.SetCameraOffset(TargetPosition);
                            if (UseRotationOffset && !UsePositionOffset) Camera.SetCameraOffset(TargetRotation);
                            else if (UsePositionOffset && UseRotationOffset) Camera.SetCameraOffset(TargetPosition, TargetRotation);

                            break;
                        }

                        Camera.SetCameraOffset(TargetOffsetObject.transform);

                        break;
                    }

                    // ZONE BLENDING LOGIC GOES HERE!
                }
                break;

            case ZoneBlendType.TargetState:
                {
                    if (!ZoneBlending)
                    {

                    }

                    Camera.SetCameraOffset(TargetOffsetObject.transform);
                }
                break;
        }
    }

    private void Start() => Camera = GameSystem.Instance.Camera;

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
