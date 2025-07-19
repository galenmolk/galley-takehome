using UnityEngine;

public interface IGrabbable
{
    void Grab(Transform grabPoint);
    void Release();
    void BeginHover();
    void EndHover();
}

public class CrateBehaviour : MonoBehaviour, IGrabbable
{
    [SerializeField] private Rigidbody crateRigidbody;
    [SerializeField] private float baseDragSpeed = 30f;
    [SerializeField] private float mouseSpeedDragFactor = 2f;
    [SerializeField] private float settleThreshold = 0.2f;
    [SerializeField] private float defaultLinearDamping = 1f;
    [SerializeField] private float grabLinearDamping = 5f;

    private Transform grabPoint;

    private Vector3 lastGrabPointPosition;
    private Vector3 dragVelocity;

    void Start()
    {
        crateRigidbody.linearDamping = defaultLinearDamping;
    }

    void FixedUpdate()
    {
        if (grabPoint == null)
        {
            return;
        }

        dragVelocity = (grabPoint.position - lastGrabPointPosition) / Time.fixedDeltaTime;

        lastGrabPointPosition = grabPoint.position;

        var toGrabPoint = grabPoint.position - transform.position;

        if (toGrabPoint.magnitude > settleThreshold)
        {
            crateRigidbody.AddForce(toGrabPoint.normalized * baseDragSpeed);
            crateRigidbody.AddForce(dragVelocity * mouseSpeedDragFactor, ForceMode.Acceleration);
        }
    }

    void IGrabbable.BeginHover()
    {
    }

    void IGrabbable.EndHover()
    {
    }

    void IGrabbable.Grab(Transform grabPoint)
    {
        crateRigidbody.useGravity = false;
        crateRigidbody.linearDamping = grabLinearDamping;
        lastGrabPointPosition = grabPoint.position;
        this.grabPoint = grabPoint;
    }

    void IGrabbable.Release()
    {
        this.grabPoint = null;

        crateRigidbody.linearDamping = defaultLinearDamping;
        crateRigidbody.useGravity = true;
    }
}
