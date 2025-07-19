using UnityEngine;

public class ShipSway : MonoBehaviour
{
    [SerializeField] private Rigidbody shipRb;

    [SerializeField] private Vector2 xSwayRange, ySwayRange, zSwayRange;

    [SerializeField] private Vector2 swayDurationRange;

    [SerializeField] private AnimationCurve swayCurve;

    private Vector3 targetRotation;

    private Vector3 startRotation;

    private float timer;
    private float swayDuration;

    private void Start()
    {
        startRotation = transform.localEulerAngles;
        SetRandomTargetRotation();
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        var t = swayCurve.Evaluate(Mathf.Clamp01(timer / swayDuration));

        var rotationThisFrame = Vector3.Lerp(startRotation, targetRotation, t);
        shipRb.rotation = Quaternion.Euler(rotationThisFrame);

        if (t >= 1f)
        {
            timer = 0f;
            swayDuration = Random.Range(swayDurationRange.x, swayDurationRange.y);
            startRotation = targetRotation;
            SetRandomTargetRotation();
        }
    }

    private void SetRandomTargetRotation() {
        targetRotation = new Vector3(
            Random.Range(xSwayRange.x, xSwayRange.y),
            Random.Range(ySwayRange.x, ySwayRange.y),
            Random.Range(zSwayRange.x, zSwayRange.y)
        );
    }
}
