using UnityEngine;

public class UpdatePlayerPos : MonoBehaviour, ITouchable
{
    [field: Header("Inherited from `ITouchable`")]
    [field: SerializeField] public bool Enabled { get; set; } = true;
    [field: SerializeField] public bool HideOnStartup { get; set; }

    [field: Header("Trigger Specific")]
    [field: SerializeField] private Transform TargetTransform;

    public void Entered(PlayerSystem Player)
    {
        if (!Player) return;
        if (!TargetTransform) return;
        Player.Warp(TargetTransform.position);
    }

    public void Left(PlayerSystem Player) { }

    public void Staying(PlayerSystem Player) { }

    private void Awake() => GetComponent<ITouchable>().SetupTrigger(gameObject);
}
