using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    [field: SerializeField] private GameObject[] CutscenePoints;
    [field: SerializeField] private float TimeInterval;
    [field: SerializeField] private float CameraSpeed;

    [field: SerializeField] private bool UseDefaultSpeed;

    public void CutsceneEnded()
    {
        print("Cutscene has concluded");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        PlayerSystem Player = other.GetComponent<PlayerSystem>();

        if (!Player.Camera) return;
        Player.Camera.BeginCutscene(CutscenePoints, TimeInterval, !UseDefaultSpeed ? CameraSpeed : -1.0f);
    }
}
