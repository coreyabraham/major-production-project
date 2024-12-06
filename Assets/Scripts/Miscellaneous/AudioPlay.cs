using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    [field: SerializeField] private AudioSource[] Sources;
	[field: Tooltip("Reference to the player model's Animator that's used to animate the character.")]
	
    public Animator Animator;

    public void PlaySteps()
    {
        foreach (AudioSource source in Sources)
            source.Play();
    }

    public void StopSteps()
    {
        foreach (AudioSource source in Sources)
            source.Stop();
    }
	
	public void ResetIdleIndex(float index)
	{
		Animator.SetFloat("RandomIdleIndex", index);
	}

}