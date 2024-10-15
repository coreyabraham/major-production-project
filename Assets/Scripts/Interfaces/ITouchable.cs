using UnityEngine;

public interface ITouchable
{
    public bool Enabled { get; set; }
    public bool HideOnStartup { get; set; }

    public virtual void TriggerEnter(PlayerSystem Player)
    {
        if (!Enabled) return;
        Entered(Player);
    }

    public virtual void TriggerStay(PlayerSystem Player)
    {
        if (!Enabled) return;
        Staying(Player);
    }

    public virtual void TriggerLeave(PlayerSystem Player)
    {
        if (!Enabled) return;
        Left(Player);
    }

    public virtual void SetupTrigger(GameObject gameObject)
    {
        if (!HideOnStartup) return;

        bool result = gameObject.TryGetComponent(out MeshRenderer component);

        if (!result)
        {
            Debug.LogWarning("ITouchable.cs | Could not find Mesh Renderer Component within: " + gameObject.name + "!");
            return;
        }

        component.enabled = false;
    }

    public void Entered(PlayerSystem Player);
    public void Left(PlayerSystem Player);
    public void Staying(PlayerSystem Player);
}
