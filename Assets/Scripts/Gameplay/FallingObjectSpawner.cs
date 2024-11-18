using System.Collections.Generic;
using UnityEngine;


public class FallingObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ClonedObject
    {
        public GameObject Object;
        public Material Material;

        public float CurrentLifetime;
        public float CurrentFadeOut;
    }

    #region Public Variables
    [field: Header("Overrides")]

    [field: Tooltip("If set as false, the spawner will cease instantiating.")]
    [field: SerializeField] bool isActive;
    [field: Tooltip("It's highly recommended that the prefabs have rigidbodies on them already, but if they can't for whatever reason, enabling this variable will attempt to add them before instantiating them.")]
    [field: SerializeField] bool autoAddRigidbody = false;
    //[field: Tooltip("It's intended for the prefabs to have rigidbodies on them. You can, however, skip the checks for them if you so wish.")]
    //[field: SerializeField] bool ignoreRigidbodyCheck;

    [field: Header("Spawning")]

    [field: Tooltip("The objects that the spawner will instantiate.")]
    [field: SerializeField] GameObject[] objects;
    [field: Tooltip("The speed at which the prefabs will be instantiated.")]
    [field: SerializeField] float spawnRate;
    [field: Tooltip("The amount of time in seconds between instantiations.")]
    [field: SerializeField] float spawnDelay;
    [field: Tooltip("The amount of prefabs that are allowed to be spawned before the spawner stops.\n\nSet to 0 for unlimited spawning.")]
    [field: SerializeField] int spawnLimit;
    [field: Tooltip("The amount of time in seconds that the newly spawned object lasts for before deletion.")]
    [field: SerializeField] float lifetime;

    [field: Header("Rotation")]

    [field: Tooltip("If using knife prefabs, angle them using hard-coded values to make them point downward.")]
    [field: SerializeField] bool useKnifeAngle;
    [field: Tooltip("Which axis the prefabs should be instantiated along. Also affects \"Use Knife Angle\".")]
    [field: SerializeField] PipeAxis axisToSpawnAlong;
    #endregion


    #region Private Variables
    float timer;
    BoxCollider box;
    Vector3 posToSpawnAt;
    Quaternion angToSpawnAt;
    [field: SerializeField] List<ClonedObject> clonedObjects = new();
    #endregion


    #region Public Functions
    public void ToggleActiveState(bool setActive)
    {
        if (isActive && (spawnLimit != 0 && clonedObjects.Count >= spawnLimit))
        {
            Debug.LogWarning("You're attempting to activate a Falling Object Spawner that has already reached its Spawn Limit! Nothing will spawn if it's activated!"); return;
        }

        isActive = setActive;
    }
    #endregion


    #region Private Functions
    private void InstantiateObject(int index)
    {
        GameObject objToSpawn = objects[index];

        float j;
        if (axisToSpawnAlong == PipeAxis.X)
        {
            j = Random.Range(box.bounds.min.z, box.bounds.max.z);
            posToSpawnAt = new(transform.position.x, transform.position.y + 0.6f, j - 0.1f);
        }
        else
        {
            j = Random.Range(box.bounds.min.x, box.bounds.max.x);
            posToSpawnAt = new(j - 0.1f, transform.position.y + 0.6f, transform.position.z);
        }

        if (useKnifeAngle && axisToSpawnAlong == PipeAxis.X) { angToSpawnAt = Quaternion.Euler(new(angToSpawnAt.x - 75, 0, 0)); }
        else if (useKnifeAngle && axisToSpawnAlong == PipeAxis.Z) { angToSpawnAt = Quaternion.Euler(new(angToSpawnAt.x - 75, -90, 0)); }
        else { angToSpawnAt = Quaternion.Euler(new(angToSpawnAt.x, 0, 0)); }

        objToSpawn.tag = "Touchable";

        if (autoAddRigidbody && !objToSpawn.GetComponent<Rigidbody>()) { objToSpawn.AddComponent<Rigidbody>(); }
        //{ Debug.LogError("Prefab doesn't have a rigidbody component! It needs a rigidbody to work properly!"); isActive = false; return; }

        GameObject clone = Instantiate(objToSpawn, posToSpawnAt, angToSpawnAt);
        Material material = null;

        bool result = clone.TryGetComponent<MeshRenderer>(out MeshRenderer renderer);
        if (result) material = renderer.material;

        clonedObjects.Add(new ClonedObject {
            Object = clone,
            Material = material,
            CurrentLifetime = 0.0f,
            CurrentFadeOut = 0.0f
        });

        timer = 0;
    }

    private bool FadeObjectOut(ClonedObject Clone)
    {
        if (Clone.CurrentFadeOut < lifetime && Clone.Material != null)
        {
            Clone.CurrentFadeOut += Time.deltaTime;

            Color targetColor = new(
                Clone.Material.color.r,
                Clone.Material.color.g,
                Clone.Material.color.b,
                0.0f
            );

            Clone.Material.color = Color.Lerp(Clone.Material.color, targetColor, Time.deltaTime);

            return false;
        }

        return true;
    }

    private void Update()
    {
        foreach (ClonedObject clone in clonedObjects)
        {
            if (clone.CurrentLifetime < lifetime)
            {
                clone.CurrentLifetime += Time.deltaTime;
                continue;
            }

            bool current = FadeObjectOut(clone);
            if (!current) continue;

            Destroy(clone.Object);
            clonedObjects.Remove(clone);
        }

        timer += Time.deltaTime * spawnRate;
        if (objects.Length <= 0 || !isActive || (spawnLimit != 0 && clonedObjects.Count >= spawnLimit)) { return; }

        if (timer >= spawnDelay)
        {
            if (objects.Length < 2) { InstantiateObject(0); return; }
            int i = Random.Range(0, objects.Length);
            InstantiateObject(i);
        }
    }


    private void Awake()
    {
        box = GetComponent<BoxCollider>();

        if (spawnRate < 0) { spawnRate = -spawnRate; }
        if (spawnDelay < 0) { spawnDelay = -spawnDelay; }
        if (spawnLimit < 0) { spawnLimit = 0; }
    }
    #endregion
}
