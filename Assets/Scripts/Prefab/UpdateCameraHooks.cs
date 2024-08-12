using UnityEngine;
using UnityEngine.Events;

public class UpdateCameraHooks : MonoBehaviour
{
    [field: SerializeField] private bool YieldMovement = true;
    [field: SerializeField] private CameraTarget[] TargetTransforms;

    public UnityEvent<CameraSystem> TriggerFinished;

    private void FinishedTransform(CameraSystem Camera) => TriggerFinished?.Invoke(Camera);

    private void OnTriggerEnter(Collider other)
    {
        PlayerSystem player = other.gameObject.GetComponentInParent<PlayerSystem>();
        if (!player) return;

        CameraSystem camera = player.Camera;
        if (!camera) return;

        if (camera.IsLerpingToPoint()) return;

        camera.SetMultipleCameraTargets(TargetTransforms, YieldMovement, 1.0f, 0.0f, FinishedTransform);
    }
}
