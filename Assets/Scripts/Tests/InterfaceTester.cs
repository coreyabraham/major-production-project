using UnityEngine;

public class InterfaceTester : MonoBehaviour, IInteractable
{
    [field: SerializeField] public float InteractionRange { get; set; }
    [field: SerializeField] private PlayerSystem Player { get; set; }

    public void Interact()
    {
        if (Vector3.Distance(Player.transform.position, this.transform.position) > InteractionRange)
        {
            Debug.Log("Interacted!... but you're too far away :(");
            return;
        }

        Debug.Log("INTERACTED!");
    }

    private void Interacted(bool result)
    {
        print(result);
        if (!result) return;
        Interact();
    }

    private void Start() => Player.Events.Interacting.AddListener(Interacted);
}
