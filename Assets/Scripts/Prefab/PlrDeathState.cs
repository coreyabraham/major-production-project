using UnityEngine;

public class PlrDeathState : MonoBehaviour, ITouchable
{
    [field: Header("Inherited from `ITouchable`")]
    [field: SerializeField] public bool Enabled { get; set; } = true;
    [field: SerializeField] public bool HideOnStartup { get; set; }

    public void Entered(Collider other)
    {
        if (!GameSystem.Instance.Player) return;
        GameSystem.Instance.Player.DeathTriggered();
    }

    public void Left(Collider other) { }

    private void Awake() => GetComponent<ITouchable>().SetupTrigger(gameObject);
}
