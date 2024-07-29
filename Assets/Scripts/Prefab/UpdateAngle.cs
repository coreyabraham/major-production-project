using UnityEngine;

public class UpdateAngle : MonoBehaviour
{
    [field: SerializeField] private CameraAxis TargetCameraAxis;

    private void OnTriggerEnter(Collider other)
    {
        PlayerSystem player = other.gameObject.GetComponentInParent<PlayerSystem>();
        if (!player) return;

        player.Camera.CameraAngle = TargetCameraAxis;
    }
}
