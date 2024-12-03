using UnityEngine;

public class EndingTrigger : MonoBehaviour, ITouchable
{
    [field: Header("ITouchable Inheritence")]
    [field: SerializeField] public bool Enabled { get; set; }
    [field: SerializeField] public bool HideOnStartup { get; set; }

    [field: Header("External References")]
    [field: SerializeField] private EndingUI UI;

    [field: Header("Timing")]
    [field: SerializeField] private float FadeTime;
    [field: SerializeField] private float FadeModifier;

    [field: Header("Colors")]
    [field: SerializeField] private Color TargetColor;

    private float CurrentFadeTime = 0.0f;

    private bool Triggered = false;
    private bool RunTimer = false;

    private Color GetColorLerp()
    {
        Color targetColor = new(
            TargetColor.r,
            TargetColor.g,
            TargetColor.b,
            1.5f
        );

        return Color.Lerp(UI.Background.color, targetColor, Time.deltaTime * FadeModifier);
    }

    public void Entered(PlayerSystem Player)
    {
        if (Triggered) return;

        Triggered = true;
        RunTimer = true;

        GameSystem.Instance.Camera.PauseMovement = true;
    }

    public void Left(PlayerSystem Player) { }

    public void Staying(PlayerSystem Player) { }

    private void Update()
    {
        if (!RunTimer) return;

        if (CurrentFadeTime < FadeTime)
        {
            CurrentFadeTime += Time.deltaTime * FadeModifier;
            UI.Background.color = GetColorLerp();
            return;
        }

        CurrentFadeTime = 0.0f;
        RunTimer = false;

        UI.EnableUI();
    }

    private void Awake() => GetComponent<ITouchable>().SetupTrigger(gameObject);
}
