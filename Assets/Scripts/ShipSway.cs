using UnityEngine;

public class ShipSway : MonoBehaviour
{
    [SerializeField] private Rigidbody shipRb;

    [SerializeField] private Vector2 xSwayRange, ySwayRange, zSwayRange;

    [SerializeField] private Vector2 swayDurationRange;

    [SerializeField] private AnimationCurve swayCurve;

    [Tooltip("Wait a small amount of time after game starts before enabling sway.")]
    [SerializeField] private float startGameDelay;

    private Vector3 targetRotation;

    private Vector3 startRotation;

    private float timer;
    private float swayDuration;
    private Vector3 baseRotation;

    private void Start()
    {
        baseRotation = transform.eulerAngles;
        startRotation = baseRotation;
        Debug.Log($"start rot: {startRotation}");
        SetRandomTargetRotation();
    }

    private void FixedUpdate()
    {
        if (Time.time < startGameDelay)
        {
            return;
        }

        timer += Time.deltaTime;
        var t = swayCurve.Evaluate(Mathf.Clamp01(timer / swayDuration));

        var rotationThisFrame = Vector3.Lerp(startRotation, targetRotation, t);
        shipRb.MoveRotation(Quaternion.Euler(rotationThisFrame));

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
        ) + baseRotation;
    }
}
