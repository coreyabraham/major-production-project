using UnityEngine;

public class UpdateMovement : MonoBehaviour, ITouchable
{
    [field: Header("Inherited from `ITouchable`")]
    [field: SerializeField] public bool Enabled { get; set; } = true;
    [field: SerializeField] public bool HideOnStartup { get; set; }

    [field: Header("Trigger Specific")]
    [field: SerializeField] private MoveType TargetMoveType;
    [field: SerializeField] private bool ResetVelocity;

    public void Entered(PlayerSystem Player) => Player.SetMoveType(TargetMoveType, ResetVelocity);

    public void Staying(PlayerSystem Player) { }

    public void Left(PlayerSystem Player) { }

    private void Awake() => GetComponent<ITouchable>().SetupTrigger(gameObject);
}
