using UnityEngine;

public class FallForPipe : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;

    [SerializeField] private Vector3 endPos;

    private bool hasBeenSteppedOn = false;
    private float timeElapsed = 0.0f;



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) { hasBeenSteppedOn = true; }
    }


    private void FixedUpdate()
    {
        if (!hasBeenSteppedOn && transform.parent.GetChild(1).name == "FallingPipe") { return; }

        timeElapsed += Time.fixedDeltaTime;

        var normalizedProgress = timeElapsed;
        var easing = curve.Evaluate(normalizedProgress);

        transform.parent.GetChild(1).localPosition = Vector3.Lerp(transform.parent.GetChild(1).localPosition, endPos, easing);

        if (transform.parent.GetChild(1).localPosition.y > -1.113f) { return; }
        transform.parent.GetChild(1).gameObject.SetActive(false);
        transform.parent.GetChild(2).gameObject.SetActive(true);
    }
}
