using UnityEngine;

public class LoadSceneEvent : MonoBehaviour, ITouchable
{
    [field: Header("Inherited from `ITouchable`")]
    [field: SerializeField] public bool Enabled { get; set; } = true;
    [field: SerializeField] public bool HideOnStartup { get; set; }

    [field: Header("Trigger Specific")]
    [field: SerializeField] private string SceneName;
    [field: SerializeField] private bool HaltPlayerMovement = true;

    private bool TriggerTouched = false;

    public void Entered(PlayerSystem Player)
    {
        if (TriggerTouched) return;
        TriggerTouched = true;
        
        if (HaltPlayerMovement) Player.SetMoveType(MoveType.None, true);

        GameSystem.Instance.RequestLoadScene(SceneName);
    }

    public void Left(PlayerSystem Player) { }

    public void Staying(PlayerSystem Player) { }

    private void Awake() => GetComponent<ITouchable>().SetupTrigger(gameObject);
}
