using UnityEngine;

public class UpdateMovement : MonoBehaviour
{
    [field: SerializeField] private MovementType TargetMoveType;
    [field: SerializeField] private bool ResetVelocity;

    private void OnTriggerEnter(Collider other)
    {
        PlayerSystem player = other.gameObject.GetComponentInParent<PlayerSystem>();
        if (!player) return;

        player.SetMovementType(TargetMoveType, ResetVelocity);
    }
}
