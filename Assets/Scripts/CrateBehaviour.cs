using DG.Tweening;
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
    [SerializeField] private MeshRenderer rend;
    [SerializeField] private float baseDragSpeed = 30f;
    [SerializeField] private float mouseSpeedDragFactor = 2f;
    [SerializeField] private float settleThreshold = 0.2f;
    [SerializeField] private float defaultLinearDamping = 1f;
    [SerializeField] private float grabLinearDamping = 5f;
    [SerializeField] private float hoverGlowFadeDuration = 0.3f;
    [SerializeField] private Ease hoverGlowEase = Ease.InOutCubic;
    [SerializeField, ColorUsage(true, true)] private Color normalColor;
    [SerializeField, ColorUsage(true, true)] private Color hoverColor;

    private Transform grabPoint;

    private Vector3 lastGrabPointPosition;
    private Vector3 dragVelocity;

    void Start()
    {
        crateRigidbody.linearDamping = defaultLinearDamping;
        rend.material = new Material(rend.sharedMaterial);
    }

    void FixedUpdate()
    {
        if (grabPoint == null || crateRigidbody == null)
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
        if (rend != null)
        {
            rend.material.EnableKeyword("_EMISSION");
            rend.material.DOKill();
            rend.material.DOColor(hoverColor, "_EmissionColor", hoverGlowFadeDuration).SetEase(hoverGlowEase);
        }
    }

    void IGrabbable.EndHover()
    {
        if (rend != null)
        {
            rend.material.DisableKeyword("_EMISSION");
            rend.material.DOKill();
            rend.material.DOColor(normalColor, "_EmissionColor", hoverGlowFadeDuration).SetEase(hoverGlowEase);
        }
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
        if (crateRigidbody == null)
        {
            return;
        }

        this.grabPoint = null;

        crateRigidbody.linearDamping = defaultLinearDamping;
        crateRigidbody.useGravity = true;
    }
}
