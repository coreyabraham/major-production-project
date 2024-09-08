using UnityEngine;
using UnityEngine.Events;

public class InterfaceTester : MonoBehaviour, IInteractable
{
    [field: SerializeField] public float InteractionRange { get; set; }
    [field: SerializeField] private PlayerSystem Player { get; set; }

    public void Interact()
    {
        print("Interacted!");
    }

    private void Interacted(bool result)
    {
        print(result);
        if (!result) return;
        Interact();
    }

    private void Start()
    {
        Player.Events.Interacting.AddListener(Interacted);
    }
}
