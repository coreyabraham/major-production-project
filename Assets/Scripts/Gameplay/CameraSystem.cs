using System;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    public PlayerSystem Player;
    public CameraAxis CameraAngle = CameraAxis.North;

    [field: SerializeField] private bool TargetPlayer = false;

    [field: SerializeField] private Vector3 CameraPositionOffset;
    [field: SerializeField] private Vector3 CameraRotationOffset;

    [field: SerializeField] private bool LerpCamera;
    [field: SerializeField] private float CameraLerpSpeed;

    [HideInInspector] public Camera main;
    
    public void CameraMove(bool Right)
    {
        int maxCount = Enum.GetNames(typeof(CameraAxis)).Length;
        int value = (int)CameraAngle;
        
        if (Right)
        {
            if (value == maxCount - 1)
            {
                CameraAngle = CameraAxis.North;
                return;
            }

            value++;
        }

        else
        {
            if (value == 0) value = maxCount - 1;
            else value--;
        }

        CameraAngle = (CameraAxis)value;
    }

    private void Awake() => main = GetComponentInChildren<Camera>();

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
        
        // TODO: Make sure that when the Camera rotates, it keeps the Player Character in focus instead of ignoring it's transform!
        float angle = DictateAngle();
        Vector3 vecRot = new(main.transform.rotation.x, angle, main.transform.rotation.z);

        Quaternion newRot = Quaternion.Euler(vecRot + CameraRotationOffset);

        if (!LerpCamera)
        {
            main.transform.SetPositionAndRotation(newPos, newRot);
            return;
        }

        main.transform.SetPositionAndRotation(
            Vector3.MoveTowards(main.transform.position, newPos, Time.fixedDeltaTime * CameraLerpSpeed), 
            Quaternion.Lerp(main.transform.rotation, newRot, Time.fixedDeltaTime * CameraLerpSpeed)
        );
    }
}
