using UnityEngine;

public class LoadingTester : MonoBehaviour, ITouchEvent
{
    [field: SerializeField] public bool TriggeringEnabled { get; set; } = true;
    [field: SerializeField] public bool PlayerExclusive { get; set; } = true;

    [field: SerializeField] private string SceneName = "Josh";

    private bool AlreadyTriggered = false;

    public void Triggered(Collider Other)
    {
        if (AlreadyTriggered) return;
        if (!Other.TryGetComponent(out PlayerSystem _)) return;

        AlreadyTriggered = true;
        GameSystem.Instance.RequestLoadScene(SceneName);
    }

    private void OnTriggerEnter(Collider other) => Triggered(other);
}
