using UnityEngine;


public class FallingObjectSpawner : MonoBehaviour
{
    #region Public Variables
    [field: Tooltip("Follows regular spawn conditions when true, overrides spawn conditions when false.")]
    /*[field: SerializeField] bool isActive;*/

    [field: Space]

  /*  [field: Tooltip("The objects that the spawner will instantiate.")]*/
    [field: SerializeField] GameObject[] objects;
    [field: SerializeField] bool alterAngle;
    [field: Tooltip("The speed at which the prefabs will be instantiated.")]
    [field: SerializeField] float spawnrate;
    [field: Tooltip("The amount of time between instantiations.")]
    [field: SerializeField] float delayBetweenSpawns;
    [field: Tooltip("Which axis the prefabs should be instantiated along.")]
    [field: SerializeField] PipeAxis axisToSpawnAlong;

    [field: Space]

    [field: Tooltip("It's intended for the prefabs to have rigidbodies on them. You can, however, skip the checks for them if you so wish.")]
    [field: SerializeField] bool ignoreRigidbodyCheck;
    #endregion


    #region Private Variables
    float timer;
    BoxCollider box;
    Vector3 posToSpawnAt;
    Quaternion angToSpawnAt;
    #endregion

   public bool isActive;

    private void InstantiateObject(int index)
    {
        GameObject objToSpawn = objects[index];

        float j;
        if (axisToSpawnAlong == PipeAxis.X)
        {
            j = Random.Range(box.bounds.min.x, box.bounds.max.x);
            posToSpawnAt = new(j - 0.1f, transform.position.y + 0.6f, transform.position.z);
        }
        else
        {
            j = Random.Range(box.bounds.min.z, box.bounds.max.z);
            posToSpawnAt = new(transform.position.x, transform.position.y + 0.6f, j - 0.1f);
        }

        if (alterAngle) { angToSpawnAt = Quaternion.Euler(new(0, 0, -75)); }

        if (!ignoreRigidbodyCheck && !objToSpawn.GetComponent<Rigidbody>())
        { Debug.LogError("Prefab doesn't have a rigidbody component! It needs a rigidbody to work properly!"); isActive = false; return; }

        Instantiate(objToSpawn, posToSpawnAt, angToSpawnAt);

        timer = 0;
    }


    private void Update()
    {
        timer += Time.deltaTime * spawnrate;
        if (objects.Length <= 0 || !isActive) { return; }

        if (timer >= delayBetweenSpawns)
        {
            if (objects.Length > 1) { InstantiateObject(0); return; }
            int i = Random.Range(0, objects.Length);
            InstantiateObject(i);
        }
    }


    private void Awake()
    {
        box = GetComponent<BoxCollider>();
    }
}
