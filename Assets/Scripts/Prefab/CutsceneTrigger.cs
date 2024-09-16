using UnityEngine;

public class CutsceneTrigger : MonoBehaviour, ITouchable
{
    [field: Header("Inherited from `ITouchable`")]
    [field: SerializeField] public bool TriggeringEnabled { get; set; }
    [field: SerializeField] public bool PlayerExclusive { get; set; }

    [field: Header("Trigger Specific")]
    [field: SerializeField] private GameObject[] CutscenePoints;
    [field: SerializeField] private float TimeInterval;
    [field: SerializeField] private float CameraSpeed;

    [field: SerializeField] private bool UseDefaultSpeed;

    public void CutsceneEnded()
    {
        print("Cutscene has concluded");
    }

    public void Touched()
    {
        CameraSystem Camera = GameSystem.Instance.Camera;
        if (!Camera) return;
        Camera.BeginCutscene(CutscenePoints, TimeInterval, !UseDefaultSpeed ? CameraSpeed : -1.0f);
    }
}
