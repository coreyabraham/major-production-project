using UnityEngine;

public class PlrDeathState : MonoBehaviour, ITouchEvent
{
    [field: SerializeField] public bool TriggeringEnabled { get; set; } = true;
    [field: SerializeField] public bool PlayerExclusive { get; set; } = true;

    public void Triggered(Collider Other)
    {
        if (!PlayerExclusive) return;
        if (!Other.TryGetComponent<PlayerSystem>(out PlayerSystem player)) return;
        player.DeathTriggered();
    }

    private void OnTriggerEnter(Collider other) => Triggered(other);
}
