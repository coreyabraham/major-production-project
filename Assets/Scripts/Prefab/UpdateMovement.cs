using UnityEngine;

public class UpdateMovement : MonoBehaviour, ITouchable
{
    [field: Header("Inherited from `ITouchable`")]
    [field: SerializeField] public bool TriggeringEnabled { get; set; } = true;
    [field: SerializeField] public bool PlayerExclusive { get; set; } = true;

    [field: Header("Trigger Specific")]
    [field: SerializeField] private MoveType TargetMoveType;
    [field: SerializeField] private bool ResetVelocity;

    public void Touched()
    {
        if (!GameSystem.Instance.Player) return;
        GameSystem.Instance.Player.SetMoveType(TargetMoveType, ResetVelocity);
    }
}
