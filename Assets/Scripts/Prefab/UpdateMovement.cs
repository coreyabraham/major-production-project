using UnityEngine;

public class UpdateMovement : MonoBehaviour
{
    [field: SerializeField] private MovementType TargetMoveType;
    [field: SerializeField] private bool ResetVelocity;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        other.GetComponent<PlayerSystem>().SetMovementType(TargetMoveType, ResetVelocity);
    }
}
