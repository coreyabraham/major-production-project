using System.Collections.Generic;
using System.Linq;

using UnityEngine;

/// <summary>
/// Goes on a trigger box and checks for the Player. It then removes the box from the player and classes them as unhidden.
/// </summary>
public class RemoveBoxFromPlayer : MonoBehaviour, ITouchable
{
    #region Public Variables
    [field: Header("References")]
    [field: SerializeField] private GameObject TargetInstance;

    [field: Header("ITouchable Inheritance")]
    [field: SerializeField] public bool Enabled { get; set; } = true;
    [field: SerializeField] public bool HideOnStartup { get; set; } = false;
    #endregion

    #region Private Variables
    bool hasTriggeredOnce = false;
    #endregion

    private bool DestroyTarget(PlayerSystem Player)
    {
        List<GameObject> children = new();

        for (int i = 0; i < Player.gameObject.transform.childCount; i++)
        {
            Transform child = Player.transform.GetChild(i);
            children.Add(child.gameObject);
        }

        GameObject target = children.Where(x => x.name == TargetInstance.name).SingleOrDefault();
        if (!target) return false;

        Destroy(target);

        return true;
    }

    public void Entered(PlayerSystem Player)
    {
        if (hasTriggeredOnce) { return; }
        
        bool result = DestroyTarget(Player);
        if (!result) return;

        Player.IsHidden = false;
        hasTriggeredOnce = true;
    }

    public void Left(PlayerSystem Player) {  }

    public void Staying(PlayerSystem Player) { }

    private void Awake() => GetComponent<ITouchable>().SetupTrigger(gameObject);
}
