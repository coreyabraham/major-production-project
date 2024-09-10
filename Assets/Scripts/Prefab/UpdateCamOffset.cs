using UnityEngine;

public class UpdateCamOffset : MonoBehaviour
{
    [field: SerializeField] private bool HideOnStartup = true;
    [field: SerializeField] private bool UseDampening = false;
    [field: SerializeField] private bool RevertOnExit = false;

    [field: Space(5.0f)]

    [field: SerializeField] private GameObject Goal;

    [field: Space(5.0f)]

    [field: SerializeField] private UpdateCamOffsetType Type;
    [field: SerializeField] private UpdateCamOffsetValue Value;

    [field: Space(5.0f)]

    [field: SerializeField] private CameraTarget Target;

    private PlayerSystem Player;
    private CameraSystem Camera;

    private CameraTarget BeforeChanges = new();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Player = other.gameObject.GetComponent<PlayerSystem>();
        if (!Player) return;

        Camera = Player.Camera;
        if (!Camera) return;

        BeforeChanges = Camera.GetCameraOffset();

        if (UseDampening) return;

        switch (Type)
        {
            case UpdateCamOffsetType.Target:
                {
                    switch (Value)
                    {
                        case UpdateCamOffsetValue.Both: Camera.SetCameraOffsets(Target); break;
                        case UpdateCamOffsetValue.Position: Camera.SetCameraOffsets(Target.position); break;
                        case UpdateCamOffsetValue.Rotation: Camera.SetCameraOffsets(Target.rotation); break;
                    }
                }
                return;

            case UpdateCamOffsetType.Default: Camera.SetToDefaultOffsets(); return;
            case UpdateCamOffsetType.Previous: Camera.RevertCameraOffsets(); return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!Player || !Camera) return;

        if (RevertOnExit)
        {
            Camera.SetCameraOffsets(BeforeChanges);
            BeforeChanges = new();
        }

        Player = null;
        Camera = null;
    }

    private void Update()
    {
        if (!UseDampening || !Player || !Camera) return;

        float Distance = Vector3.Distance(
            Player.gameObject.transform.position,
            Goal.transform.position
        );

        // CameraTarget.cs : `position`, `rotation`

        CameraTarget CamTransform = Camera.GetCameraOffset();

        CameraTarget target = new()
        {
            position = Vector3.Lerp(CamTransform.position, Target.position, Time.deltaTime),
            rotation = Quaternion.Lerp(CamTransform.rotation, Target.rotation, Time.deltaTime)
        };

        print(Distance);

        Camera.SetCameraOffsets(target);
    }

    private void Awake()
    {
        if (!HideOnStartup) return;

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (!renderer) return;

        renderer.enabled = false;
    }
}
