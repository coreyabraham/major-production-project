using System.Collections.Generic;
using UnityEngine;

public class PlrDeathState : MonoBehaviour, ITouchable
{
    #region Inspector Variables
    [field: Header("Settings // Standard")]
    [field: SerializeField] private DeathType DeathType = DeathType.Default;
    [field: SerializeField] private bool PrintMessages = false;
    [field: SerializeField] private float DeathDelayTime = 0.5f;
    [field: SerializeField] private bool DisableControlOnDeath = false;

    [field: Header("Settings // Debouncing")]
    [field: SerializeField] private bool UseDebounce = true;
    [field: SerializeField] private float DebounceTime = 1.0f;

    [field: Header("Inherited from `ITouchable`")]
    [field: SerializeField] public bool Enabled { get; set; } = true;
    [field: SerializeField] public bool HideOnStartup { get; set; }
    #endregion

    #region Private Variables
    private float CurrentDebounceTime = 0.0f;
    private float CurrentDelayTime = 0.0f;
    private bool DebounceStarted = false;
    private bool DelayStarted = false;

    private PlayerSystem PlaySys;

    readonly private Dictionary<DeathType, System.Action> MethodLookup = new();
    #endregion

    #region Death Methods
    // <TODO> : MOVE THESE TO THEIR OWN FILE!
    private void PrayedDeath()
    {
        Debug.Log(name + " | Implement `Prayed` Death State here!");
    }

    private void BurnedDeath()
    {
        Debug.Log(name + " | Implement `Burned` Death State here!");
    }

    private void DrownedDeath()
    {
        Debug.Log(name + " | Implement `Drowned` Death State here!");
    }
    #endregion

    #region ITouchable Inheritance
    public void Entered(PlayerSystem Player)
    {
        if (!Player || DebounceStarted || Player.IsHidden) return;

        PlaySys = Player; 
        DebounceStarted = DelayStarted = true;
    }

    public void Left(PlayerSystem Player) { }

    public void Staying(PlayerSystem Player)
    {
        if (CurrentDelayTime < DeathDelayTime) { return; }
        Player.DeathTriggered();
    }
    #endregion


    private void TriggerDeath(PlayerSystem Player)
    {
        bool result = MethodLookup.TryGetValue(DeathType, out System.Action method);

        if (!result && PrintMessages) Debug.Log(name + " | There was no `DeathType` Method found for DeathType: " + DeathType.GetName(typeof(DeathType), DeathType) + "!");
        else if (result) method();

        Player.DeathTriggered();
    }


    #region Unity Methods
    private void Update()
    {
        if (DelayStarted)
        {
            CurrentDelayTime += Time.deltaTime;
            if (DisableControlOnDeath) { PlaySys.SetMoveType(MoveType.None, false); }
            if (CurrentDelayTime >= DeathDelayTime)
            {
                TriggerDeath(PlaySys);

                CurrentDelayTime = 0.0f;
                DelayStarted = false;
            }
        }

        if (!UseDebounce || !DebounceStarted) return;

        if (CurrentDebounceTime < DebounceTime)
        {
            CurrentDebounceTime += Time.deltaTime;
            return;
        }

        CurrentDebounceTime = 0.0f;
        DebounceStarted = false;
    }

    private void Awake()
    {
        MethodLookup.Add(DeathType.Prayed, PrayedDeath);
        MethodLookup.Add(DeathType.Burned, BurnedDeath);
        MethodLookup.Add(DeathType.Drowned, DrownedDeath);

        GetComponent<ITouchable>().SetupTrigger(gameObject);
    }
    #endregion
}
