using UnityEngine;

public class PlrDeathState : MonoBehaviour, ITouchable
{
    [field: Header("Inherited from `ITouchable`")]
    [field: SerializeField] public bool TriggeringEnabled { get; set; } = true;
    [field: SerializeField] public bool PlayerExclusive { get; set; } = true;

    public void Touched()
    {
        if (!GameSystem.Instance.Player) return;
        GameSystem.Instance.Player.DeathTriggered();
    }
}
