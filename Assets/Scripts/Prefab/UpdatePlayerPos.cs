using UnityEngine;

public class UpdatePlayerPos : MonoBehaviour, ITouchable
{
    [field: Header("Inherited from `ITouchable`")]
    [field: SerializeField] public bool Enabled { get; set; } = true;

    [field: Header("Trigger Specific")]
    [field: SerializeField] private Transform TargetTransform;

    public void Entered(Collider other)
    {
        if (!GameSystem.Instance.Player) return;
        if (!TargetTransform) return;

        GameSystem.Instance.Player.Warp(TargetTransform.position);
    }
}
