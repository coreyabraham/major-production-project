using UnityEngine;

public class UpdateMovement : MonoBehaviour, ITouchEvent
{
    [field: SerializeField] public bool TriggeringEnabled { get; set; } = true;
    [field: SerializeField] public bool PlayerExclusive { get; set; } = true;

    [field: SerializeField] private MovementType TargetMoveType;
    [field: SerializeField] private bool ResetVelocity;

    public void Triggered(Collider Other)
    {
        if (!PlayerExclusive) return;

        PlayerSystem player = Other.gameObject.GetComponentInParent<PlayerSystem>();
        if (!player) return;

        player.SetMovementType(TargetMoveType, ResetVelocity);
    }

    private void OnTriggerEnter(Collider other) => Triggered(other);
}
