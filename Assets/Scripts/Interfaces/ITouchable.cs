using UnityEngine;

public interface ITouchable
{
    public bool Enabled { get; set; }
    public bool HideOnStartup { get; set; }

    public virtual void TriggerEnter(Collider other)
    {
        if (!Enabled) return;
        Entered(other);
    }

    public virtual void TriggerLeave(Collider other)
    {
        if (!Enabled) return;
        Left(other);
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

    public void Entered(Collider other);
    public void Left(Collider other);
}
