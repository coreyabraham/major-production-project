using UnityEngine;

public class PlrDeathState : MonoBehaviour
{
    [field: SerializeField] private Transform RespawnTransform;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<PlayerSystem>(out PlayerSystem player)) return;

        player.DeathTriggered();

        // Here temporarily until `DeathTriggered()` is finished!
        player.WarpToPosition(RespawnTransform.position);
        player.Camera.ForceCameraBackToPlayer();
    }
}
