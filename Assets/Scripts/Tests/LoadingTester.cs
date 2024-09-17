using UnityEngine;

public class LoadingTester : MonoBehaviour, ITouchable
{
    [field: Header("Inherited from `ITouchable`")]
    [field: SerializeField] public bool Enabled { get; set; } = true;

    [field: Header("Trigger Specific")]
    [field: SerializeField] private string SceneName = "Josh";

    private bool AlreadyTriggered = false;

    public void Entered(Collider other)
    {
        if (!AlreadyTriggered) return;
        AlreadyTriggered = true;

        GameSystem.Instance.RequestLoadScene(SceneName);
    }
}
