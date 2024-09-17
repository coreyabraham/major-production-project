using UnityEngine;

public class UpdateMovement : MonoBehaviour, ITouchable
{
    [field: Header("Inherited from `ITouchable`")]
    [field: SerializeField] public bool Enabled { get; set; } = true;

    [field: Header("Trigger Specific")]
    [field: SerializeField] private MoveType TargetMoveType;
    [field: SerializeField] private bool ResetVelocity;

    public void Entered(Collider other)
    {
        if (!GameSystem.Instance.Player) return;
        GameSystem.Instance.Player.SetMoveType(TargetMoveType, ResetVelocity);
    }
}
