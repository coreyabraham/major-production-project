using UnityEngine;
using UnityEngine.Events;

public class UpdateCameraHook : MonoBehaviour
{
    [field: SerializeField] private bool YieldMovement = true;
    [field: SerializeField] private Transform TargetCameraTransform;

    public UnityEvent TriggerFinished;

    private void FinishedTransform() => TriggerFinished?.Invoke();

    private void OnTriggerEnter(Collider other)
    {
        PlayerSystem player = other.gameObject.GetComponentInParent<PlayerSystem>();
        if (!player) return;

        CameraSystem camera = player.Camera;
        if (!camera) return;

        if (!TargetCameraTransform)
        {
            camera.TargetPlayer = true;
            return;
        }

        camera.SetCameraTransform(TargetCameraTransform, YieldMovement, FinishedTransform);
    }
}
