using UnityEngine;

public class UpdateCamOffset : MonoBehaviour
{
    public enum UpdateCamOffsetType
    {
        Target = 0,
        Default,
        Previous
    }

    [field: SerializeField] private CameraTarget Target;
    [field: SerializeField] private UpdateCamOffsetType Type;

    [field: SerializeField] private bool HideOnStartup = true;

    private void Awake()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (!renderer) return;

        renderer.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerSystem player = other.gameObject.GetComponentInParent<PlayerSystem>();
        if (!player) return;

        CameraSystem camera = player.Camera;
        if (!camera) return;

        switch (Type)
        {
            case UpdateCamOffsetType.Target: camera.SetCameraOffsets(Target); return;
            case UpdateCamOffsetType.Default: camera.SetToDefaultOffsets(); return;
            case UpdateCamOffsetType.Previous: camera.RevertCameraOffsets(); return;
        }
    }
}
