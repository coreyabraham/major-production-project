using UnityEngine;

public class TEMP_removeBox : MonoBehaviour
{
    public GameObject destroyThis;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerSystem playSys = other.GetComponent<PlayerSystem>();
            playSys.IsHidden = false;

            Destroy(playSys.gameObject.transform.GetChild(1).gameObject);
            Destroy(destroyThis);
            Destroy(this.gameObject);
        }
    }
}
