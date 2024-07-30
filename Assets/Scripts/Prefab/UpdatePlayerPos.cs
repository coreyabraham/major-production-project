using UnityEngine;

public class UpdatePlayerPos : MonoBehaviour
{
    [field: SerializeField] private Transform TargetTransform;

    private void OnTriggerEnter(Collider other)
    {
        PlayerSystem player = other.gameObject.GetComponentInParent<PlayerSystem>();

        if (!player) return;
        if (!transform) return;

        player.WarpToPosition(TargetTransform.position);
    }
}
