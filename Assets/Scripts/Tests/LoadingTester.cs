using UnityEngine;

public class LoadingTester : MonoBehaviour, ITouchable
{
    [field: Header("Inherited from `ITouchable`")]
    [field: SerializeField] public bool Enabled { get; set; } = true;
    [field: SerializeField] public bool HideOnStartup { get; set; }

    [field: Header("Trigger Specific")]
    [field: SerializeField] private string SceneName = "Josh";

    public void Entered(Collider other) => GameSystem.Instance.RequestLoadScene(SceneName);

    public void Left(Collider other) { }

    private void Awake() => GetComponent<ITouchable>().SetupTrigger(gameObject);
}
