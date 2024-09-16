using UnityEngine;

public class InterfaceTester : MonoBehaviour, IInteractable
{
    [field: SerializeField] public float InteractionRange { get; set; }
    [field: SerializeField] public bool IgnoreInteractionRange { get; set; }

    public void Interacted()
    {
        print("You did it!");
    }
}
