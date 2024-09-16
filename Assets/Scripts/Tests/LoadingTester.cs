using UnityEngine;

public class LoadingTester : MonoBehaviour, ITouchable
{
    [field: Header("Inherited from `ITouchable`")]
    [field: SerializeField] public bool TriggeringEnabled { get; set; } = true;
    [field: SerializeField] public bool PlayerExclusive { get; set; } = true;

    [field: Header("Trigger Specific")]
    [field: SerializeField] private string SceneName = "Josh";

    private bool AlreadyTriggered = false;

    public void Touched()
    {
        if (!AlreadyTriggered) return;
        AlreadyTriggered = true;

        GameSystem.Instance.RequestLoadScene(SceneName);
    }
}
