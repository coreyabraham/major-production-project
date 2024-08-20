using UnityEngine;

public class TEMP_ACfanblade : MonoBehaviour
{
    [field: SerializeField] bool poweredOn;
    [field: SerializeField] GameObject blade1, blade2;

    private void Update()
    {
        if (poweredOn)
        {
            blade1.transform.Rotate(0, 0, -1000 * Time.deltaTime);
            blade2.transform.Rotate(0, 0, -1000 * Time.deltaTime);
        }
    }
}
