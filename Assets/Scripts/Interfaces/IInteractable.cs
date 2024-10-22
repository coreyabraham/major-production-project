using UnityEngine;

public interface IInteractable
{
    public float InteractionRange { get; set; }
    public bool IgnoreInteractionRange { get; set; }
    
    public virtual void Interact(GameObject This, PlayerSystem Interactor = null)
    {
        if (!Interactor) Interactor = GameSystem.Instance.Player;
        
        if (!IgnoreInteractionRange)
        {
            if (Vector3.Distance(This.transform.position, Interactor.transform.position) > InteractionRange) return;
        }

        Interacted(Interactor);
;    }

    public void Interacted(PlayerSystem Player);
}