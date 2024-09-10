using UnityEngine;

public class TakeawayBoxTrigger : MonoBehaviour
{
    #region Public Variables
    [field: SerializeField] GameObject takeawayBox;

    [field: Header("Position and Rotation")]

    [field: Tooltip("The position that the box will apply to itself when interacted with by the Player.\n\nNote that the box will become a child of the Player, so the coordinates provided should be relatively close to (0, 0, 0).")]
    [field: SerializeField] Vector3 posOnPlayer;
    [field: Tooltip("The rotation that the box will apply to itself when interacted with by the Player.\n\nNote that the box will become a child of the Player, so the Quaternion provided should be based around that.")]
    [field: SerializeField] Quaternion rotOnPlayer;
    #endregion

    #region Private Variables
    private PlayerSystem playerSystem;
    #endregion

    #region Functions - Private
    private void PutBoxOnPlayer()
    {
        takeawayBox.transform.parent = playerSystem.gameObject.transform;
        takeawayBox.transform.SetLocalPositionAndRotation(posOnPlayer, rotOnPlayer);
        playerSystem.IsHidden = true;
    }


    private void Update()
    {
        if (playerSystem == null) return;

        // this needs to be replaced, just hard coding it to make sure it works.
        if (!playerSystem.IsHidden && Input.GetKeyDown(KeyCode.E))
        {
            PutBoxOnPlayer();
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
