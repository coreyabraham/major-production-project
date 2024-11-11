using UnityEngine;

[System.Serializable]
public struct IInteractableData
{
    public GameObject Parent;
    public IInteractable Interactable;
}

[System.Serializable]
public struct ITouchableData
{
    public GameObject Parent;
    public ITouchable Touchable;
}