using UnityEngine;

public class UpdateMovement : MonoBehaviour
{
    [field: SerializeField] private MoveType TargetMoveType;
    [field: SerializeField] private bool ResetVelocity;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        other.GetComponent<PlayerSystem>().SetMoveType(TargetMoveType, ResetVelocity);
    }
}
