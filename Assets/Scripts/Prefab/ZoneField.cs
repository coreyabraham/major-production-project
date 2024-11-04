using UnityEngine;
using UnityEngine.Events;

public class ZoneField : MonoBehaviour, ITouchable
{
    // TEMPORARY ENUM PLACEMENT, PLACE IN ANOTHER ENUM SCRIPT!
    public enum ZoneValueType
    {
        Middle = 0,
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

    private float CurrentValue;
    private float TriggerSize;

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

        float selection = 0.0f;

        switch (LocalScaleType)
        {
            case CartesianCoords.X: selection = transform.position.x - CachedPlayer.transform.position.x; break;
            case CartesianCoords.Y: selection = transform.position.y - CachedPlayer.transform.position.y; break;
            case CartesianCoords.Z: selection = transform.position.z - CachedPlayer.transform.position.z; break;
        }

        float distanceFromTriggerCentre = Mathf.Abs(selection);

        float blendAmount = 1 - distanceFromTriggerCentre / TriggerSize;
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
    }
}
