using UnityEngine;
using UnityEngine.Events;

public class ZoneField : MonoBehaviour, ITouchable
{
    // TEMPORARY ENUM PLACEMENT, PLACE IN ANOTHER ENUM SCRIPT!
    public enum ZoneValueType
    {
        Middle = 0,
        InverseMiddle,
        Start,
        End
    }

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

    [field: Header("Events")]
    public UnityEvent<float> ValueUpdated;

    private float CurrentValue, TriggerSize;
    private Vector3 Start, End;

    private PlayerSystem CachedPlayer;

    public void TEST(float Value) => print(Value.ToString());

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

        switch (ValueType)
        {
            case ZoneValueType.Start:
                switch (LocalScaleType)
                {
                    case CartesianCoords.X: selection = Start.x - CachedPlayer.transform.position.x; break;
                    case CartesianCoords.Y: selection = Start.y - CachedPlayer.transform.position.y; break;
                    case CartesianCoords.Z: selection = Start.z - CachedPlayer.transform.position.z; break;
                }

                selection /= 2;
                break;
            
            case ZoneValueType.End:
                switch (LocalScaleType)
                {
                    case CartesianCoords.X: selection = End.x - CachedPlayer.transform.position.x; break;
                    case CartesianCoords.Y: selection = End.y - CachedPlayer.transform.position.y; break;
                    case CartesianCoords.Z: selection = End.z - CachedPlayer.transform.position.z; break;
                }

                selection /= 2;
                break;
        }

        if (selection <= float.MinValue)
        {
            switch (LocalScaleType)
            {
                case CartesianCoords.X: selection = transform.position.x - CachedPlayer.transform.position.x; break;
                case CartesianCoords.Y: selection = transform.position.y - CachedPlayer.transform.position.y; break;
                case CartesianCoords.Z: selection = transform.position.z - CachedPlayer.transform.position.z; break;
            }
        }

        float distanceFromTriggerCentre = Mathf.Abs(selection);
        float blendAmount = 0.0f;

        switch (ValueType)
        {
            case ZoneValueType.Middle: blendAmount = 1 - distanceFromTriggerCentre / TriggerSize; break;
            case ZoneValueType.InverseMiddle: blendAmount = distanceFromTriggerCentre / TriggerSize; break;
            case ZoneValueType.Start: blendAmount = distanceFromTriggerCentre / TriggerSize; break;
            case ZoneValueType.End: blendAmount = distanceFromTriggerCentre / TriggerSize; break;
        }

        CurrentValue = LerpCurve.Evaluate(blendAmount);
        ValueUpdated?.Invoke(CurrentValue);
    }

    private void Awake()
    {
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

        GameObject startPointer = new();
        startPointer.name = "Start";
        startPointer.transform.SetParent(gameObject.transform);
        startPointer.transform.position = Start;

        GameObject endPointer = new();
        endPointer.name = "End";
        endPointer.transform.SetParent(gameObject.transform);
        endPointer.transform.position = End;
    }
}
