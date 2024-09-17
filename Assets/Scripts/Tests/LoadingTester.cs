using UnityEngine;

public class LoadingTester : MonoBehaviour, ITouchable
{
    [field: Header("Inherited from `ITouchable`")]
    [field: SerializeField] public bool Enabled { get; set; } = true;

    [field: Header("Trigger Specific")]
    [field: SerializeField] private string SceneName = "Josh";

    public void Entered(Collider other) => GameSystem.Instance.RequestLoadScene(SceneName);
}
