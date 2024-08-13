using UnityEngine;

public class ChineseBoxTrigger : MonoBehaviour
{
    #region Public Variables
    [field: SerializeField] GameObject ChineseBox;
    #endregion

    #region Private Variables
    private PlayerSystem playerSystem;
    #endregion

    #region Functions - Private
    private void MakePlayerHidden()
    {
        ChineseBox.transform.parent = playerSystem.gameObject.transform;
        ChineseBox.transform.SetLocalPositionAndRotation(new(0, 0.09f, -0.04f), new(0, 0, 180, 0));
        playerSystem.IsHidden = true;
    }


    private void Update()
    {
        if (playerSystem == null) return;

        // this needs to be replaced, just hard coding it to make sure it works.
        if (!playerSystem.IsHidden && Input.GetKeyDown(KeyCode.E))
        {
            MakePlayerHidden();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSystem = other.GetComponent<PlayerSystem>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSystem = null;
        }
    }
    #endregion
}
