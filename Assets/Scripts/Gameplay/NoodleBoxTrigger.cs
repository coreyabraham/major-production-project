using UnityEngine;

public class NoodleBoxTrigger : MonoBehaviour
{
    [field: SerializeField] GameObject NoodleBox;
    private PlayerSystem playerSystem;

    private void MakePlayerHidden()
    {
        NoodleBox.transform.parent = playerSystem.gameObject.transform;
        NoodleBox.transform.localPosition = new(0, 0.14f, -0.01f);
        playerSystem.IsHidden = true;
    }


    private void Update()
    {
        if (playerSystem == null) return;

        // this needs to be replaced, just hard coding it to make sure it works.
        if (!playerSystem.IsHidden && Input.GetKeyDown(KeyCode.E))
        {
            MakePlayerHidden();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSystem = other.GetComponent<PlayerSystem>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSystem = null;
        }
    }
}
