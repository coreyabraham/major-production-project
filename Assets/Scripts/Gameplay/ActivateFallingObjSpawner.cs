using UnityEngine;

public class ActivateFallingObjSpawner : MonoBehaviour
{
    #region Public Variables
    [Header("References to Spawners")]

    [field: Tooltip("References to all of the spawners that will be enabled when the player walks into this trigger.")]
    [field: SerializeField] FallingObjectSpawner[] spawnerScript;

    [Header("Bools")]

    [field: Tooltip("Only allow the player to activate this trigger once. Subsequent attempts to activate this trigger will be ignored.")]
    [field: SerializeField] bool onlyActivateOnce = true;
    [field: Tooltip("Set all of the referenced spawners' \"Is Active\" variables to false when the scene starts.")]
    [field: SerializeField] bool disableReferencesOnStart;
    #endregion

    #region Private Variables
    bool hasActivatedOnce = false;
    #endregion

    #region Private Functions
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || spawnerScript.Length == 0 && (onlyActivateOnce && !hasActivatedOnce)) { return; }

        if (spawnerScript.Length == 1)
        {
            spawnerScript[0].ToggleActiveState(true);
        }
        else
        {
            for (int i = 0; i < spawnerScript.Length; i++)
            {
                spawnerScript[i].ToggleActiveState(true);
            }
        }

        hasActivatedOnce = true;
    }

    private void Start()
    {
        if (disableReferencesOnStart && spawnerScript.Length != 0)
        {
            if (spawnerScript.Length == 1)
            {
                spawnerScript[0].ToggleActiveState(false);
            }
            else
            {
                for (int i = 0; i < spawnerScript.Length; i++)
                {
                    spawnerScript[i].ToggleActiveState(false);
                }
            }
        }
    }
    #endregion
}
