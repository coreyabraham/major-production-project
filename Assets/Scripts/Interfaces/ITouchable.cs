using UnityEngine;

public interface ITouchable
{
    public bool TriggeringEnabled { get; set; }
    public bool PlayerExclusive { get; set; }

    public virtual void Triggered(GameObject Collision)
    {
        if (!TriggeringEnabled) return;

        if (PlayerExclusive)
        {
            if (!Collision.TryGetComponent(out PlayerSystem _)) return;
        }

        Touched();
    }

    public virtual void Touched() { }
}
