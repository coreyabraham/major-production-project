using UnityEngine.Events;

public interface IController
{
    public UnityEvent<bool> Event { get; set; }
    public abstract void Evaluate();
}
