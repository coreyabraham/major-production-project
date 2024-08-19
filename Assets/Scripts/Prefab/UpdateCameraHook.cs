using UnityEngine;
using UnityEngine.Events;

public class UpdateCameraHook : MonoBehaviour
{
    [field: SerializeField] private bool YieldMovement = true;
    [field: SerializeField] private Transform TargetCameraTransform;

    public UnityEvent<CameraSystem> TriggerFinished;

    private void FinishedTransform(CameraSystem Camera) => TriggerFinished?.Invoke(Camera);

    private void OnTriggerEnter(Collider other)
    {
        PlayerSystem player = other.gameObject.GetComponentInParent<PlayerSystem>();
        if (!player) return;

        CameraSystem camera = player.Camera;
        if (!camera) return;

        if (camera.IsLerpingToPoint()) return;

        if (!TargetCameraTransform)
        {
            camera.TargetPlayer = true;
            return;
        }

        camera.SetCameraTarget(TargetCameraTransform, YieldMovement, FinishedTransform);
    }
}
