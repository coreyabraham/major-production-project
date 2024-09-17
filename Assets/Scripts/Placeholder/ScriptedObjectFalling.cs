using UnityEngine;

public class ScriptedObjectFalling : MonoBehaviour
{
    #region Public Variables
    [field: Header("References")]

    [field: Tooltip("Specify which rigidbody to use.\n\nNote that if this variable is not set, it will assume that the rigidbody intended for use is on this same object and will attempt to retrieve a reference to it.")]
    [field: SerializeField] Rigidbody rb;

    [field: Header("Type of Force")]

    [field: Tooltip("The direction that the force should be applied to when interacted with.")]
    [field: SerializeField] AxisValueType axis;
    [field: Tooltip("The force that will be applied to this object when interacted with.\n\nNegative values will automatically be converted to positive ones. If the object is travelling along the right axis but in the wrong direction, \"Axis\" needs to be changed, not this value.")]
    [field: SerializeField] float force;

    [field: Header("Context-Specific")]

    [field: Tooltip("If the rigidbody's \"Use Gravity\" variable needs to be enabled during an interaction, set this to true.")]
    [field: SerializeField] bool considerGravity;
    [field: Tooltip("If the collider(s) of this object need to be disabled, list them here and they will be on being interacted with.\n\nIf this isn't needed, leave the list size as 0 and it will be skipped.")]
    [field: SerializeField] Collider[] colliderToDestroy;
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
            if (colliderToDestroy.Length >= 1)
            {
                for (int i = 0; i < colliderToDestroy.Length; i++)
                {
                    Destroy(colliderToDestroy[i]);
                }
            }
            if (considerGravity) { rb.isKinematic = false; rb.useGravity = true; }

            rb.AddForce(force * dir, ForceMode.Impulse);
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

        if (considerGravity) { rb.isKinematic = true; rb.useGravity = false; }
    }
}
