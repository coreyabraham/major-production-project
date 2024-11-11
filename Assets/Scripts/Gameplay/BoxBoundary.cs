using UnityEngine;

public class BoxBoundary : MonoBehaviour
{
    #region Public Variables
    [field: Tooltip("Disables this box's mesh renderer when the scene starts.")]
    [field: SerializeField] bool HideOnStart;
    #endregion

    #region Private Variables
    PlayerSystem playSys;
    #endregion

    #region Private Functions
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) { return; }
        if (!playSys)
        {
            playSys = other.GetComponent<PlayerSystem>();
            playSys.TogglePullState(false);
            playSys.ToggleGrabbing(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) { return; }
        if (playSys)
        {
            playSys.ToggleGrabbing(true);
            playSys = null;
        }
    }

    private void Start()
    {
        if (HideOnStart) { GetComponent<MeshRenderer>().enabled = false; }
    }
    #endregion
}
