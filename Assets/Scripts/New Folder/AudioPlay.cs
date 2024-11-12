using JetBrains.Annotations;
using UnityEngine;

public class AudioPlay : MonoBehaviour
{
   public AudioSource aud;
    Animator animator;
    public AudioSource walk;

    // Start is called before the first frame update
    void Start()
    {
     //   aud = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //// This checks if the animation is currently playing
        //if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk") || aud.isPlaying)
        //{
        //    // If the animation is not playing or the sound is not playing, stop the sound
        //    aud.Stop();
        //}
    }

    // This method will be called from the Animation Event at the specific frame
  //  public void PlaySound()
   // {
        //if (!aud.isPlaying)  // Only play the sound if it's not already playing

        //      {
        //          if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk") || aud.isPlaying)
        //          {
        //              aud.Play();
        //              Debug.Log("playSound");

        //          }
        //          else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || aud.isPlaying)
        //          {
        //              aud.Stop();
        //              Debug.Log("playNone");
        //          }

        void PlaySteps()
        {
            walk.Play();
            Debug.Log("PLAY");

        }
    void StopSteps()
    {
        walk.Stop();
        Debug.Log("STOP");

    }
}

