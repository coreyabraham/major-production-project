using UnityEngine;

public class InterfaceTester : MonoBehaviour, IInteractable
{
    [field: Header("IInteractable Inheritance")]
    [field: SerializeField] public float InteractionRange { get; set; }
    [field: SerializeField] public bool IgnoreInteractionRange { get; set; }

    private AudioSource Source;

    public void Interacted(PlayerSystem Player)
    {
        if (Source.isPlaying) Source.Stop();
        Source.Play();
    }

    private void Awake() => Source = GetComponent<AudioSource>();
}
