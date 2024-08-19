using UnityEngine;

public class UpdateCamOffset : MonoBehaviour, ITouchEvent
{
    public enum UpdateCamOffsetType
    {
        Target = 0,
        Default,
        Previous
    }
    [field: SerializeField] public bool TriggeringEnabled { get; set; } = true;
    [field: SerializeField] public bool PlayerExclusive { get; set; } = true;

    [field: SerializeField] private CameraTarget Target;
    [field: SerializeField] private UpdateCamOffsetType Type;

    [field: SerializeField] private bool HideOnStartup = true;

    public void Triggered(Collider Other)
    {
        if (!PlayerExclusive) return;

        PlayerSystem player = Other.gameObject.GetComponentInParent<PlayerSystem>();
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

    private void Awake()
    {
        if (!HideOnStartup) return;

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (!renderer) return;

        renderer.enabled = false;
    }

    private void OnTriggerEnter(Collider other) => Triggered(other);
}
