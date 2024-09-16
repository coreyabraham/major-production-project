using UnityEngine;

public class ScriptedObjectFalling : MonoBehaviour
{
    #region Public Variables
    [field: Header("Reference to Rigidbody")]

    [field: Tooltip("Specify which rigidbody to use.\n\nNote that if this variable is not set, it will assume that the rigidbody intended for use is on this same object and will attempt to retrieve a reference to it.")]
    [field: SerializeField] Rigidbody rb;

    [field: Header("Type of Force")]

    [field: Tooltip("The direction that the force should be applied to when interacted with.")]
    [field: SerializeField] AxisValueType axis;
    [field: Tooltip("The force that will be applied to this object when interacted with.\n\nNegative values will automatically be converted to positive ones. If the object is travelling along the right axis but in the wrong direction, \"Axis\" needs to be changed, not this value.")]
    [field: SerializeField] float force;
    [field: Tooltip("If the rigidbody's \"Use Gravity\" variable needs to be enabled during an interaction, set this to true.")]
    [field: SerializeField] bool considerGravity;
    
    [field: Header("External Interaction")]

    [field: Tooltip("If another object with this script touches this object, should it apply force? This should be true on the object that will be affected by another object, not the one causing it.\n\nNote that all objects intended to use this script should all share the same tag.")]
    [field: SerializeField] bool isAffectedByKin;
    #endregion

    #region Private Variables
    Vector3 dir;
    #endregion


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || (other.CompareTag(tag) && isAffectedByKin))
        {
            rb.AddForce(force * dir, ForceMode.Impulse);

            if (considerGravity) { rb.useGravity = true; }
        }
    }


    private void Update()
    {
        switch (axis)
        {
            case AxisValueType.XPos: dir = Vector3.right; break;
            case AxisValueType.YPos: dir = Vector3.up; break;
            case AxisValueType.ZPos: dir = Vector3.forward; break;
            case AxisValueType.XNeg: dir = Vector3.left; break;
            case AxisValueType.YNeg: dir = Vector3.down; break;
            case AxisValueType.ZNeg: dir = Vector3.back; break;
        }
    }


    private void Start()
    {
        if (force < 0) { force = -force; }
        if (!rb) { rb = GetComponent<Rigidbody>(); }

        if (considerGravity) { rb.useGravity = false; }
    }
}
