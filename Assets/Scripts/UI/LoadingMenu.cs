using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class LoadingMenu : MonoBehaviour
{
    [field: Header("Assets")]
    [field: SerializeField] private GameObject Canvas;

    [field: Space(2.5f)]

    [field: SerializeField] private Image Background;
    [field: SerializeField] private Animator SpriteAnimator;

    private bool Loading = false;
    private bool UIRequested = false;

    private float ArtificialTimer = 0.0f;
    readonly private float MaxArtificialTimer = 0.0f;

    private float CurrentProgress = 0.0f;

    public void ToggleLoadingScreen(bool Toggle)
    {
        Canvas.SetActive(Toggle);
        SpriteAnimator.gameObject.SetActive(Toggle);
    }

    public void RequestUI()
    {
        UIRequested = true;
        ToggleLoadingScreen(true);
    }

    public void LoadingStarted() => Loading = true;

    public void LoadingFinished()
    {
        Loading = false;
        UIRequested = false;

        ArtificialTimer = 0.0f;
        CurrentProgress = 0.0f;

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
    }

    private void Start() => Canvas.SetActive(false);
}
