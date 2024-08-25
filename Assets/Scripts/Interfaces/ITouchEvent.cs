using UnityEngine;

public interface ITouchEvent
{
    bool TriggeringEnabled { get; set; }
    bool PlayerExclusive { get; set; }

    void Triggered(Collider Other);
}
