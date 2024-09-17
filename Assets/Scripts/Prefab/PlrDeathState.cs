using UnityEngine;

public class PlrDeathState : MonoBehaviour, ITouchable
{
    [field: Header("Inherited from `ITouchable`")]
    [field: SerializeField] public bool Enabled { get; set; } = true;

    public void Entered(Collider other)
    {
        if (!GameSystem.Instance.Player) return;
        GameSystem.Instance.Player.DeathTriggered();
    }
}
