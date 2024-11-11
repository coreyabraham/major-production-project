using UnityEngine;
using UnityEngine.Events;

public class ZoneField : MonoBehaviour, ITouchable
{
    [field: Header("ITouchable Inheritance")]
    [field: SerializeField] public bool Enabled { get; set; } = true;
    [field: SerializeField] public bool HideOnStartup { get; set; }

    [field: Header("Value Options")]
    [field: SerializeField] private ZoneValueType ValueType = ZoneValueType.Middle;

    [field: Header("Scaling Options")]
    [field: SerializeField] private CartesianCoords LocalScaleType = CartesianCoords.X;
    [field: SerializeField] private float TransformModifier = 0.5f;

    [field: Header("Miscellaneous")]
    [field: SerializeField] private AnimationCurve LerpCurve;
    [field: SerializeField] private bool PlaceDebugVisuals;

    [field: Header("Debug")]
    [field: SerializeField] private Mesh DebugMesh;
    [field: SerializeField] private Material DebugMaterial;

    [field: Header("Events")]
    public UnityEvent<float> ValueUpdated;

    private float CurrentValue, TriggerSize;
    private Vector3 Start, End;

    private PlayerSystem CachedPlayer;

    public void Entered(PlayerSystem Player)
    {
        if (CachedPlayer) return;
        CachedPlayer = Player;
    }

    public void Left(PlayerSystem Player)
    {
        if (!CachedPlayer) return;
        CachedPlayer = null;
    }

    public void Staying(PlayerSystem Player) { }

    private void Update()
    {
        if (!CachedPlayer) return;

        float selection = float.MinValue;
        Vector3 selector = Vector3.zero;

        switch (ValueType)
        {
            case ZoneValueType.Start:
                selector = (Start - CachedPlayer.transform.position) / 2;
                break;
            
            case ZoneValueType.End:
                selector = (End - CachedPlayer.transform.position) / 2;
                break;
        }

        if (selector == Vector3.zero) selector = transform.position - CachedPlayer.transform.position;

        switch (LocalScaleType)
        {
            case CartesianCoords.X: selection = selector.x; break;
            case CartesianCoords.Y: selection = selector.y; break;
            case CartesianCoords.Z: selection = selector.z; break;
        }
        
        float distanceFromTriggerCentre = Mathf.Abs(selection);
        float blendAmount = distanceFromTriggerCentre / TriggerSize;

        if (ValueType == ZoneValueType.Middle)
            blendAmount = 1 - distanceFromTriggerCentre / TriggerSize;

        CurrentValue = LerpCurve.Evaluate(blendAmount);
        ValueUpdated?.Invoke(CurrentValue);
    }

    private void Awake()
    {
        GetComponent<ITouchable>().SetupTrigger(gameObject);

        switch (LocalScaleType)
        {
            case CartesianCoords.X: TriggerSize = transform.localScale.x * TransformModifier; break;
            case CartesianCoords.Y: TriggerSize = transform.localScale.y * TransformModifier; break;
            case CartesianCoords.Z: TriggerSize = transform.localScale.z * TransformModifier; break;
        }

        BoxCollider collider = GetComponent<BoxCollider>();

        if (!collider)
        {
            Debug.LogWarning(name + " | No `BoxCollider` Component is attached! Please make sure there's a Box Collider attached so measurements can be properly aligned!");
            return;
        }

        Vector3 Center = collider.bounds.center;

        Start = new() 
        {
            x = collider.bounds.min.x,
            y = Center.y,
            z = Center.z
        };

        End = new()
        {
            x = collider.bounds.max.x,
            y = Center.y,
            z = Center.z
        };

        if (!PlaceDebugVisuals) return;

        GameObject startPointer = new();
        startPointer.name = "Start";
        startPointer.transform.SetParent(gameObject.transform);
        startPointer.transform.position = Start;

        GameObject endPointer = new();
        endPointer.name = "End";
        endPointer.transform.SetParent(gameObject.transform);
        endPointer.transform.position = End;

        GameObject[] objs = new GameObject[2];
        objs[0] = startPointer;
        objs[1] = endPointer;

        foreach (GameObject obj in objs)
        {
            MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
            meshFilter.mesh = DebugMesh;

            MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
            meshRenderer.material = DebugMaterial;

            obj.transform.localScale = new(0.015f, 0.1f, 0.1f);
        }
    }
}
