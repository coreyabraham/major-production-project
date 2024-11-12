using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class FadeTransition : MonoBehaviour
{
    [field: Header("Asset References")]
    [field: SerializeField] private Canvas Canvas;
    [field: SerializeField] private Image Frame;

    [field: Header("Settings // Numbers")]
    [field: SerializeField] private float FadeInTime = 1.0f;
    [field: SerializeField] private float FadeOutTime = 1.0f;
    [field: SerializeField] private float HoldInterval = 3.0f;
    [field: SerializeField] private float IntervalModifier = 1.0f;

    [field: Header("Settings // Miscellaneous")]
    [field: SerializeField] private Color StartColor;
    [field: SerializeField] private Color TargetColor;

    private bool RunTransition = false;
    private bool FadeState = false; // true = Faded in, false = Faded out
    private bool InMiddlePoint = false;

    private float Current = 0.0f;
    private Color TrueStartColor;

    private bool CalculateTime(float Max)
    {
        if (Current < Max)
        {
            Current += IntervalModifier * Time.deltaTime;
            return false;
        }

        Current = 0.0f;
        return true;
    }

    private Color GetColorLerp()
    {
        Color targetColor = new(
            TargetColor.r,
            TargetColor.g,
            TargetColor.b,
            FadeState ? 0.0f : 1.0f
        );

        return Color.Lerp(Frame.color, targetColor, IntervalModifier * Time.deltaTime);
    }

    private void Update()
    {
        if (!RunTransition) return;

        if (!FadeState)
        {
            FadeState = CalculateTime(FadeInTime);
            Frame.color = GetColorLerp();

            InMiddlePoint = FadeState;
            
            if (InMiddlePoint)
                Frame.color = TargetColor;

            return;
        }

        if (InMiddlePoint)
        {
            if (CalculateTime(HoldInterval))
            {
                InMiddlePoint = false;
                GameSystem.Instance.Camera.ForceCameraTarget();
            }

            return;
        }

        Frame.color = GetColorLerp();

        if (!CalculateTime(FadeOutTime)) return;

        FadeState = false;
        RunTransition = false;

        Frame.color = TrueStartColor;
    }

    private void PlayerDied(PlayerSystem Player) => RunTransition = true;

    private void Start()
    {
        TrueStartColor = new(
            StartColor.r,
            StartColor.g,
            StartColor.b,
            0.0f
        );

        Frame.color = TrueStartColor;
        GameSystem.Instance.Events.PlayerDied.AddListener(PlayerDied);
    }
}
