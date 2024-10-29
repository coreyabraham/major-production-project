using UnityEngine;

public class TakeawayBoxTrigger : MonoBehaviour, IInteractable
{
    #region Public Variables
    [field: SerializeField] GameObject takeawayBox;

    [field: Header("Position and Rotation")]
    [field: Tooltip("The position that the box will apply to itself when interacted with by the Player.\n\nNote that the box will become a child of the Player, so the coordinates provided should be relatively close to (0, 0, 0).")]
    [field: SerializeField] Vector3 posOnPlayer;
    [field: Tooltip("The rotation that the box will apply to itself when interacted with by the Player.\n\nNote that the box will become a child of the Player, so the Quaternion provided should be based around that.")]
    [field: SerializeField] Quaternion rotOnPlayer;

    [field: Header("IInteractable Inheritence")]
    public float InteractionRange { get; set; } = 1.0f;
    public bool IgnoreInteractionRange { get; set; } = false;
    #endregion

    #region Functions - Private
    public void Interacted(PlayerSystem Player)
    {
        Debug.Log(Player.IsHidden);
        if (Player.IsHidden) return;

        takeawayBox.transform.parent = Player.gameObject.transform;
        takeawayBox.transform.SetLocalPositionAndRotation(posOnPlayer, rotOnPlayer);

        Player.IsHidden = true;
    }
    #endregion
}
