using UnityEngine;

public interface ITouchable
{
    public bool Enabled { get; set; }

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

    public virtual void Entered(Collider other) { }
    public virtual void Left(Collider other) { }
}
