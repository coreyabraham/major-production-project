using UnityEngine;

public class UpdatePlayerPos : MonoBehaviour
{
    [field: SerializeField] private Transform TargetTransform;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerSystem player = other.GetComponent<PlayerSystem>();

        if (!player) return;
        if (!TargetTransform) return;

        player.Warp(TargetTransform.position);
    }
}
