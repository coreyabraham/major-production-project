using UnityEngine;

public class BoxScript : MonoBehaviour
{
    #region Variables - Public
    [field: Header("Box Properties")]

    [field: Tooltip("The distance that the box should be from the player when grabbing.\n\nValues that are too low will cause the box to teleport over the player, while values that are too high will push it away too far.")]
    [field: SerializeField] float grabDistanceFromPlayer;
    #endregion


    #region Functions - Public
    public float GetGrabDistance() => grabDistanceFromPlayer / 10f;
    #endregion

    #region Functions - Private
    private void Start()
    {
        if (grabDistanceFromPlayer <= 0)
        {
            Debug.LogWarning("Properties for Grabbable Box have not been set correctly!");
            if (grabDistanceFromPlayer < 0) { grabDistanceFromPlayer = -grabDistanceFromPlayer; }
        }
    }
    #endregion
}
