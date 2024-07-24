using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [field: SerializeField] private PlayerSystem Player;
    [field: SerializeField] private bool TargetPlayer = false;

    [field: SerializeField] private Vector3 CameraPositionOffset;
    [field: SerializeField] private Quaternion CameraRotationOffset;

    [field: SerializeField] private bool LerpCamera;
    [field: SerializeField] private float CameraLerpSpeed;

    [HideInInspector] public Camera main;

    private void Start()
    {
        main = GetComponentInChildren<Camera>();
    }

    private void FixedUpdate()
    {
        if (!TargetPlayer) return;

        Transform charTransform = Player.Character.gameObject.transform;
        Vector3 newPos = charTransform.position + CameraPositionOffset;

        if (!LerpCamera)
        {
            main.transform.SetPositionAndRotation(newPos, CameraRotationOffset);
            return;
        }

        main.transform.SetPositionAndRotation(Vector3.MoveTowards(main.transform.position, newPos, Time.fixedDeltaTime * CameraLerpSpeed), Quaternion.Lerp(main.transform.rotation, charTransform.rotation, Time.fixedDeltaTime * CameraLerpSpeed));
    }
}
