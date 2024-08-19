using UnityEngine;

public class CutsceneTester : MonoBehaviour, ITouchEvent
{
    [field: SerializeField] public bool TriggeringEnabled { get; set; } = true;
    [field: SerializeField] public bool PlayerExclusive { get; set; } = true;

    [field: SerializeField] private GameObject[] CutscenePoints;
    [field: SerializeField] private float TimeInterval;
    [field: SerializeField] private float CameraSpeed;

    [field: SerializeField] private bool UseDefaultSpeed;

    public void CutsceneEnded()
    {
        print("Cutscene has concluded");
    }

    public void Triggered(Collider Other)
    {
        if (!PlayerExclusive) return;

        PlayerSystem Player = Other.GetComponent<PlayerSystem>();

        if (!Player) return;
        if (!Player.Camera) return;

        Player.Camera.BeginCutscene(CutscenePoints, TimeInterval, !UseDefaultSpeed ? CameraSpeed : -1.0f);
    }

    private void OnTriggerEnter(Collider other) => Triggered(other);
}
