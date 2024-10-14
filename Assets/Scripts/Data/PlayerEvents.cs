using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct PlayerEvents
{
    public UnityEvent<Vector2> Moving;
    public UnityEvent<bool> Jumping;
    public UnityEvent<bool> Scurrying;
    public UnityEvent<bool> Climbing;
    public UnityEvent<bool> Interacting;
}