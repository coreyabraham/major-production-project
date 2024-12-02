using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonWrapper : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [field: SerializeField] private AudioClip ClickSound;
    [field: SerializeField] private AudioClip HoverSound;

    private AudioSource ClickSource;
    private AudioSource HoverSource;

    public void OnPointerClick(PointerEventData eventData)
    {
        ClickSource.Play();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (HoverSource.isPlaying) return;
        HoverSource.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!HoverSource.isPlaying) return;
        HoverSource.Stop();
    }

    private void Start()
    {
        ClickSource = AudioHandler.Instance.GetSource(ClickSound.name, true);
        HoverSource = AudioHandler.Instance.GetSource(HoverSound.name, true);

        if (ClickSource == null) ClickSource = AudioHandler.Instance.CreateGlobalSource(ClickSound, AudioType.Sound);
        if (HoverSource == null) HoverSource = AudioHandler.Instance.CreateGlobalSource(HoverSound, AudioType.Sound);
    }
}
