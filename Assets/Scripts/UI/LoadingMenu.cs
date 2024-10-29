using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class LoadingMenu : MonoBehaviour
{
    [field: Header("Assets")]
    [field: SerializeField] private GameObject Canvas;

    [field: Space(2.5f)]

    [field: SerializeField] private Image Background;
    [field: SerializeField] private TMP_Text Label;
    [field: SerializeField] private Slider Bar;

    [field: Header("Settings")]
    [field: SerializeField] private float LerpingModifier = 1.0f;

    private bool Loading = false;
    private bool UIRequested = false;

    private float ArtificialTimer = 0.0f;
    readonly private float MaxArtificialTimer = 0.0f;

    private float TimeElapsed = 0.0f;
    private float CurrentProgress = 0.0f;

    public void ToggleLoadingScreen(bool Toggle) => Canvas.SetActive(Toggle);

    public void RequestUI()
    {
        UIRequested = true;
        ToggleLoadingScreen(true);
    }

    public void LoadingStarted() => Loading = true;

    public void LoadingFinished()
    {
        print("Time Spent loading: " + TimeElapsed.ToString());

        Loading = false;
        UIRequested = false;

        TimeElapsed = 0.0f;
        ArtificialTimer = 0.0f;
        CurrentProgress = 0.0f;

        Bar.value = Bar.minValue;
        Label.text = "Progress: " + Bar.minValue.ToString() + "%";

        ToggleLoadingScreen(false);
    }

    public void LoadingUpdated(float Progress)
    {
        if (!Loading) return;
        CurrentProgress = Progress;
    }

    private void Update()
    {
        if (UIRequested)
        {
            if (ArtificialTimer < MaxArtificialTimer)
            {
                ArtificialTimer += Time.deltaTime;
                return;
            }
            
            ArtificialTimer = 0.0f;
            UIRequested = false;

            GameSystem.Instance.Events.LoadingUIFinished?.Invoke();
        }

        if (!Loading) return;

        Bar.value = Mathf.Lerp(Bar.value, CurrentProgress * 2, LerpingModifier * Time.deltaTime);
        Label.text = "Progress: " + Mathf.Round(CurrentProgress * 100.0f).ToString() + "%";

        TimeElapsed += Time.deltaTime;
    }

    private void Start() => Canvas.SetActive(false);
}
