using UnityEngine;

// Used for playing audio on certain hazards.
// Serves as a hub of sorts for all audio clips for a given object or hazard.
// May not be compatible with everything. Ask programmers first.
public class PlaySound : MonoBehaviour
{
    #region Public Variables
    [field: SerializeField] AudioSource[] audioClips;

    [field: Tooltip("Sets all referenced audio clips to disabled when the scene starts. This ensures they won't accidentally be left on.")]
    [field: SerializeField] bool disableOnStart = true;
    #endregion


    #region Public Functions
    public void PlaySoundOnce(int index) => audioClips[index].enabled = true;
    public void StopSoundOnce(int index) => audioClips[index].enabled = false;
    #endregion


    #region Private Functions
    private void Start()
    {
        if (!disableOnStart) { return; }
        for (int i = 0; i < audioClips.Length; i++) { audioClips[i].enabled = false; }
    }
    #endregion
}
