using UnityEngine;

public class EndingTrigger : MonoBehaviour, ITouchable
{
    [field: Header("ITouchable Inheritence")]
    [field: SerializeField] public bool Enabled { get; set; }
    [field: SerializeField] public bool HideOnStartup { get; set; }

    [field: Header("External References")]
    [field: SerializeField] private EndingUI UserInterface;

    [field: Header("Timing")]
    [field: SerializeField] private float FadeTime;
    [field: SerializeField] private float FadeModifier;

    private float CurrentFadeTime = 0.0f;

    private bool Triggered = false;
    private bool RunTimer = false;

    private void FadingComplete()
    {

    }

    private void FadeColour()
    {
        // TODO: IMPLEMENT THIS!
    }
    public void Entered(PlayerSystem Player)
    {
        if (Triggered) return;


        Triggered = true;
        RunTimer = true;
    }

    public void Left(PlayerSystem Player) { }

    public void Staying(PlayerSystem Player) { }

    private void Update()
    {
        if (!RunTimer) return;

        if (CurrentFadeTime < FadeTime)
        {
            CurrentFadeTime += Time.deltaTime * FadeModifier;
            return;
        }

        CurrentFadeTime = 0.0f;
        RunTimer = false;
    }

    private void Awake() => GetComponent<ITouchable>().SetupTrigger(gameObject);
}
