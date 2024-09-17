using UnityEngine;

public class TEMP_ACfanblade : MonoBehaviour
{
    public bool poweredOn;
    [field: SerializeField] GameObject blade1, blade2;
    public AudioSource fanOn;
    public AudioSource fanOff;

    private void Update()
    {
        if (poweredOn)
        {
            blade1.transform.Rotate(0, 0, -1000 * Time.deltaTime);
            blade2.transform.Rotate(0, 0, -1000 * Time.deltaTime);
           
        }
    }
    private void Start()
    {
        fanOn.enabled = true;
        fanOn.Play();
        fanOff.enabled = false;
    }
}
