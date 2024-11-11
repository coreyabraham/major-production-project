using UnityEngine;

public class TakeawayBoxTrigger : MonoBehaviour, IInteractable
{
    #region Private Variables
    [field: Header("IInteractable Inheritence")]
    [field: SerializeField] public float InteractionRange { get; set; } = 1.0f;
    [field: SerializeField] public bool IgnoreInteractionRange { get; set; } = false;

    /*[field: Header("Dependencies")]
    [field: SerializeField] GameObject takeawayBox;*/

    [field: Header("Position and Rotation")]
    [field: Tooltip("The position that the box will apply to itself when interacted with by the Player.\n\nNote that the box will become a child of the Player, so the coordinates provided should be relatively close to (0, 0, 0).")]
    [field: SerializeField] Vector3 posOnPlayer;
    [field: Tooltip("The rotation that the box will apply to itself when interacted with by the Player.\n\nNote that the box will become a child of the Player, so the Quaternion provided should be based around that.")]
    [field: SerializeField] Quaternion rotOnPlayer;
    #endregion

    #region Functions - Private
    public void Interacted(PlayerSystem Player)
    {
        if (Player.IsHidden) return;

        transform.parent.parent = Player.gameObject.transform;
        transform.parent.SetLocalPositionAndRotation(posOnPlayer, rotOnPlayer);

        if (GetComponent<BoxCollider>()) { Destroy(GetComponent<BoxCollider>()); }

        Player.IsHidden = true;
    }

    private void OnEnable() => GameSystem.Instance.CacheInteractable(gameObject, GetComponent<IInteractable>());
    #endregion
}
