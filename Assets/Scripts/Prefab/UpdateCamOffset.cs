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

    private bool PlayerInTrigger = false;

    private CameraTarget BeforeChanges = new();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        BeforeChanges = Camera.GetCameraOffset();
        PlayerInTrigger = true;

        if (UseDampening) return;

        switch (Type)
        {
            case UpdateCamOffsetType.Target:
                {
                    switch (Value)
                    {
                        case UpdateCamOffsetValue.Both: Camera.SetCameraOffset(Target); break;
                        case UpdateCamOffsetValue.Position: Camera.SetCameraOffset(Target.position); break;
                        case UpdateCamOffsetValue.Rotation: Camera.SetCameraOffset(Target.rotation); break;
                    }
                }
                return;

            case UpdateCamOffsetType.Default: Camera.SetToDefaultOffsets(); return;
            case UpdateCamOffsetType.Previous: Camera.RevertCameraOffsets(); return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (RevertOnExit)
        {
            Camera.SetCameraOffset(BeforeChanges);
            BeforeChanges = new();
        }

        PlayerInTrigger = false;
    }

    private void Update()
    {
        if (!UseDampening || !PlayerInTrigger) return;

        /*
            TODO:
                1. Get the Distance between where the Player is and the `Goal` GameObject
                2. Caclulate the Position and Rotation that the Camera should currently be using between the current `CameraOffset` and the target `CameraOffset`

            Notes:
                - Lerping may not be a good way to solve this due to `SetCameraOffset()` Setting the internal Camera Offset for the `CameraSystem.cs` Script
                  and actively using lerping with the current camera's transform with the `CameraOffset` added on top
                - This system should't be all that complex to add, however with the current setup of `CameraSystem.cs` combined with this logic below, it's proved
                  to be a real struggle, if this system doesn't make it into Alpha, then hopefully it'll be obvious as to why!
         */

        float Distance = Vector3.Distance(
            Player.gameObject.transform.position,
            Goal.transform.position
        );

        CameraTarget Current = Camera.GetCameraOffset();

        Target.position.x = Mathf.Lerp(Current.position.x, Target.position.x, Distance / Time.deltaTime);

        //Target.position = Vector3.Lerp(Current.position, Target.position, Distance / Time.deltaTime);
        //Target.rotation = Quaternion.Lerp(Current.rotation, Target.rotation, Distance / Time.deltaTime);

        Camera.SetCameraOffset(Target);
    }

    private void Start()
    {
        Player = GameSystem.Instance.Player;
        Camera = GameSystem.Instance.Camera;
    }

    private void Awake()
    {
        if (!HideOnStartup) return;

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (!renderer) return;

        renderer.enabled = false;
    }
}
