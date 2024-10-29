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

    public void Entered(PlayerSystem Player)
    {
        if (hasTriggeredOnce) { return; }

        Player.IsHidden = false;
        Destroy(Player.gameObject.transform.GetChild(2).gameObject);

        hasTriggeredOnce = true;
    }

    public void Left(PlayerSystem Player) {  }

    public void Staying(PlayerSystem Player) { }
}
