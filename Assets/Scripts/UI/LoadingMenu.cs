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
    private float MaxArtificialTimer = 3.0f;

    private float TimeElapsed = 0.0f;

    public void ToggleLoadingScreen(bool Toggle) => Canvas.SetActive(Toggle);

    public void RequestUI()
    {
        UIRequested = true;
        ToggleLoadingScreen(true);
    }

    public void LoadingStarted() => Loading = true;

    public void LoadingFinished()
    {
        Loading = false;

        print("Time Spent loading: " + TimeElapsed.ToString());
        TimeElapsed = 0.0f;

        UIRequested = false;

        ArtificialTimer = 0.0f;
        MaxArtificialTimer = 3.0f;

        // Here temporarily until animation work is finished!
        ToggleLoadingScreen(false);
    }

    public void LoadingUpdated(float Progress)
    {
        print(Progress);
        Bar.value = Mathf.Lerp(Bar.value, Progress, Time.deltaTime * LerpingModifier);
        Label.text = "Progress: " + (Progress * 100.0f).ToString() + "%";
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
        TimeElapsed += Time.deltaTime;
    }

    private void Start() => Canvas.SetActive(false);
}
