using UnityEngine;

public class CutsceneTrigger : MonoBehaviour, ITouchable
{
    [field: Header("Inherited from `ITouchable`")]
    [field: SerializeField] public bool Enabled { get; set; }
    [field: SerializeField] public bool HideOnStartup { get; set; }

    [field: Header("Trigger Specific")]
    [field: SerializeField] private GameObject[] CutscenePoints;
    [field: SerializeField] private float TimeInterval;
    [field: SerializeField] private float CameraSpeed;

    [field: SerializeField] private bool UseDefaultSpeed;
    [field: SerializeField] private bool UseWarpTransform;
    [field: SerializeField] CameraTarget WarpTransform;

    private PlayerSystem CachedPlayer;

    public void CutsceneEnded()
    {
        print("Cutscene has concluded");
        if (UseWarpTransform) CachedPlayer.Warp(WarpTransform.position, WarpTransform.rotation);
        CachedPlayer = null;
    }

    public void Entered(PlayerSystem Player)
    {
        CameraSystem Camera = GameSystem.Instance.Camera;
        if (!Camera) return;
        
        CachedPlayer = Player;
        Camera.BeginCutscene(CutscenePoints, TimeInterval, !UseDefaultSpeed ? CameraSpeed : -1.0f);
    }

    public void Left(PlayerSystem Player) { }

    public void Staying(PlayerSystem Player) { }

    private void Awake() => GetComponent<ITouchable>().SetupTrigger(gameObject);
}
