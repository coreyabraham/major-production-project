using UnityEngine;

public class UpdatePlayerPos : MonoBehaviour, ITouchable
{
    [field: Header("Inherited from `ITouchable`")]
    [field: SerializeField] public bool TriggeringEnabled { get; set; } = true;
    [field: SerializeField] public bool PlayerExclusive { get; set; } = true;

    [field: Header("Trigger Specific")]
    [field: SerializeField] private Transform TargetTransform;

    public void Touched()
    {
        if (!GameSystem.Instance.Player) return;
        if (!TargetTransform) return;

        GameSystem.Instance.Player.Warp(TargetTransform.position);
    }
}
