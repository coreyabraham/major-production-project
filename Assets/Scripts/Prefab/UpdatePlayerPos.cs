using UnityEngine;

public class UpdatePlayerPos : MonoBehaviour, ITouchEvent
{
    [field: SerializeField] public bool TriggeringEnabled { get; set; } = true;
    [field: SerializeField] public bool PlayerExclusive { get; set; } = true;

    [field: SerializeField] private Transform TargetTransform;

    public void Triggered(Collider Other)
    {
        if (!PlayerExclusive) return;

        PlayerSystem player = Other.gameObject.GetComponentInParent<PlayerSystem>();

        if (!player) return;
        if (!transform) return;

        player.Warp(TargetTransform.position);
    }

    private void OnTriggerEnter(Collider other) => Triggered(other);
}
