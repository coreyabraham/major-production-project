public interface IToggleable : IController
{
    public float InteractionRange { get; set; }
    public virtual void Interact() => Evaluate();
}
