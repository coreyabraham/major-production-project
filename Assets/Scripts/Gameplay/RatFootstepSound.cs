using UnityEngine;

/// <summary>
/// Footsteps should not be handled like this, but for the sake of keeping footstep sounds in, it can stay for now.
/// </summary>
public class RatFootstepSound : MonoBehaviour
{
    [field: SerializeField] AudioSource footstepsSound;
    PlayerSystem playSys;

    void Update()
    {
        if (!(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) || !playSys.IsPlayerGrounded())
        {
            if (footstepsSound.enabled) { footstepsSound.enabled = false; }
            return; 
        }
        footstepsSound.enabled = true;
    }

    private void Start()
    {
        playSys = footstepsSound.gameObject.GetComponent<PlayerSystem>();
        footstepsSound.enabled = false;
    }
}