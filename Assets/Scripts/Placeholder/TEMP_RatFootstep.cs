using UnityEngine;

public class TEMP_RatFootstep : MonoBehaviour
{
    public AudioSource footstepsSound;
    PlayerSystem playSys;

    void Update()
    {
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && !playSys.IsPlayerJumping())
        {
            
            footstepsSound.enabled = true;
        }
        else
        {
            footstepsSound.enabled = false;
        }
    }


    private void Start()
    {
        playSys = footstepsSound.gameObject.GetComponent<PlayerSystem>();
    }
}