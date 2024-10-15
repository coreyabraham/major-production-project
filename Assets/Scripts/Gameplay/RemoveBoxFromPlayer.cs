using UnityEngine;

/// <summary>
/// Goes on a trigger box and checks for the Player. It then removes the box from the player and classes them as unhidden.
/// </summary>
public class RemoveBoxFromPlayer : MonoBehaviour, ITouchable
{
    [field: Header("ITouchable Inheritance")]
    public bool Enabled { get; set; } = true;
    public bool HideOnStartup { get; set; } = false;

    bool hasTriggeredOnce = false;

    public void Entered(Collider other)
    {
        if (hasTriggeredOnce) { return; }

        GameSystem.Instance.Player.IsHidden = false;
        Destroy(GameSystem.Instance.Player.gameObject.transform.GetChild(1).gameObject);

        hasTriggeredOnce = true;
    }

    public void Left(Collider other) {  }
}
