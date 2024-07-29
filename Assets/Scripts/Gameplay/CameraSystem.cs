using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [field: SerializeField] private PlayerSystem Player;
    [field: SerializeField] private bool TargetPlayer = false;

    [field: SerializeField] private Vector3 CameraPositionOffset;
    [field: SerializeField] private Vector3 CameraRotationOffset;

    [field: SerializeField] private bool LerpCamera;
    [field: SerializeField] private float CameraLerpSpeed;

    [field: SerializeField] private CameraAxis CameraAngle = CameraAxis.North;

    [HideInInspector] public Camera main;
    
    private void Start() => main = GetComponentInChildren<Camera>();

    private float DictateAngle()
    {
        float angle = 0.0f;

        switch (CameraAngle)
        {
            case CameraAxis.North: angle = 0.0f; break;
            case CameraAxis.NorthEast: angle = 45.0f; break;
            case CameraAxis.East: angle = 90.0f; break;
            case CameraAxis.SouthEast: angle = 135.0f; break;
            case CameraAxis.South: angle = 180.0f; break;
            case CameraAxis.SouthWest: angle = 225.0f; break;
            case CameraAxis.West: angle = 270.0f; break;
            case CameraAxis.NorthWest: angle = 315.0f; break;
        }

        return angle;
    }

    private void FixedUpdate()
    {
        if (!TargetPlayer) return;

        Transform charTransform = Player.Character.gameObject.transform;
        Vector3 newPos = charTransform.position + CameraPositionOffset;
        
        float angle = DictateAngle();
        Vector3 vecRot = new(main.transform.rotation.x, angle, main.transform.rotation.z);

        Quaternion newRot = Quaternion.Euler(vecRot + CameraRotationOffset);

        if (!LerpCamera)
        {
            main.transform.SetPositionAndRotation(newPos, newRot);
            return;
        }

        main.transform.SetPositionAndRotation(Vector3.MoveTowards(main.transform.position, newPos, Time.fixedDeltaTime * CameraLerpSpeed), Quaternion.Lerp(main.transform.rotation, newRot, Time.fixedDeltaTime * CameraLerpSpeed));
    }
}
