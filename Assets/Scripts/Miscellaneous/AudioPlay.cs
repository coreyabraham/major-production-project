using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    [field: SerializeField] private AudioSource[] Sources;
	
    private Animator Animator;

	void Start()
	{
		Animator = gameObject.GetComponent<Animator>();
	}

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