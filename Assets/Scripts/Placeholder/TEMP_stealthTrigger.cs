using UnityEngine;

public class TEMP_stealthTrigger : MonoBehaviour
{
    #region Private Variables
    private PlayerSystem playerSystem;
    #endregion

    #region Functions - Private
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSystem = other.GetComponent<PlayerSystem>();
            playerSystem.IsHidden = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSystem.IsHidden = false;
            playerSystem = null;
        }
    }
    #endregion
}
