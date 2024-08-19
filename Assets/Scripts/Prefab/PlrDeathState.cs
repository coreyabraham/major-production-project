using UnityEngine;

public class PlrDeathState : MonoBehaviour, ITouchEvent
{
    [field: SerializeField] public bool TriggeringEnabled { get; set; } = true;
    [field: SerializeField] public bool PlayerExclusive { get; set; } = true;

    [field: SerializeField] private Transform RespawnTransform;

    public void Triggered(Collider Other)
    {
        if (!PlayerExclusive) return;

        if (!Other.TryGetComponent<PlayerSystem>(out PlayerSystem player)) return;

        player.DeathTriggered();

        // Here temporarily until `DeathTriggered()` is finished!
        player.WarpToPosition(RespawnTransform.position);
        //player.Camera.ForceCameraBackToPlayer();
    }

    private void OnTriggerEnter(Collider other) => Triggered(other);
}
