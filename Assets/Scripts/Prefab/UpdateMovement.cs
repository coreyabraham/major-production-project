using UnityEngine;

public class UpdateMovement : MonoBehaviour
{
    [field: SerializeField] private MovementType TargetMoveType;

    private void OnTriggerEnter(Collider other)
    {
        PlayerSystem player = other.gameObject.GetComponentInParent<PlayerSystem>();
        if (!player) return;

        player.MoveType = TargetMoveType;
    }
}
