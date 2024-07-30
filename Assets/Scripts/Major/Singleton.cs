using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance { get => (T)_manager; private set => _manager = value; }
    private static Singleton<T> _manager = null;
    private void Awake()
    {
        if (!_manager)
        {
            _manager = this;
            Initialize();
        }
    }
    protected abstract void Initialize();
}
