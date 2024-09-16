using UnityEngine;

public interface IInteractable
{
    public float InteractionRange { get; set; }
    public bool IgnoreInteractionRange { get; set; }
    
    public virtual void Interact(GameObject This, GameObject Interactor = null)
    {
        if (!Interactor) Interactor = GameSystem.Instance.Player.gameObject;
        
        if (!IgnoreInteractionRange)
        {
            if (Vector3.Distance(This.transform.position, Interactor.transform.position) > InteractionRange) return;
        }

        Interacted();
;    }

    public virtual void Interacted() { }
}