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
    private const string EmissionProp = "_EMISSION";
    private const string EmissionColorProp = "_EmissionColor";

    [Header("Physics Settings")]
    [SerializeField] private float baseDragSpeed = 30f;

    [Tooltip("How much the mouse speed affects the force added to the object while grabbing.")]
    [SerializeField] private float mouseSpeedDragFactor = 2f;

    [Tooltip("Force will only be added to the object if its distance from the target point is greatr than this value.")]
    [SerializeField] private float settleThreshold = 0.2f;
    [SerializeField] private float defaultLinearDamping = 1f;

    [Tooltip("Allows the object to use a different linear damping value while being grabbed.")]
    [SerializeField] private float grabLinearDamping = 5f;

    [Header("Hover Glow Settings")]
    [SerializeField] private float hoverGlowFadeDuration = 0.3f;
    [SerializeField] private Ease hoverGlowEase = Ease.InOutCubic;
    [SerializeField, ColorUsage(true, true)] private Color normalColor;
    [SerializeField, ColorUsage(true, true)] private Color hoverColor;

    [Header("References")]
    [SerializeField] private Rigidbody crateRigidbody;
    [SerializeField] private MeshRenderer meshRenderer;

    private Transform grabPoint;
    private Vector3 lastGrabPointPosition;
    private Vector3 dragVelocity;

    private void Start()
    {
        crateRigidbody.linearDamping = defaultLinearDamping;
        meshRenderer.material = new Material(meshRenderer.sharedMaterial);
    }

    private void FixedUpdate()
    {
        if (grabPoint == null || crateRigidbody == null)
        {
            return;
        }

        // Determine how fast the mouse moved between ticks.
        dragVelocity = (grabPoint.position - lastGrabPointPosition) / Time.fixedDeltaTime;

        lastGrabPointPosition = grabPoint.position;

        var toGrabPoint = grabPoint.position - transform.position;

        if (toGrabPoint.magnitude > settleThreshold)
        {
            // Separate AddForce calls for additional flexibility, if needed.
            crateRigidbody.AddForce(toGrabPoint.normalized * baseDragSpeed);
            crateRigidbody.AddForce(dragVelocity * mouseSpeedDragFactor, ForceMode.Acceleration);
        }
    }

    void IGrabbable.BeginHover()
    {
        if (meshRenderer != null)
        {
            meshRenderer.material.EnableKeyword(EmissionProp);
            meshRenderer.material.DOKill();
            meshRenderer.material.DOColor(hoverColor, EmissionColorProp, hoverGlowFadeDuration).SetEase(hoverGlowEase);
        }
    }

    void IGrabbable.EndHover()
    {
        if (meshRenderer != null)
        {
            meshRenderer.material.DisableKeyword(EmissionProp);
            meshRenderer.material.DOKill();
            meshRenderer.material.DOColor(normalColor, EmissionColorProp, hoverGlowFadeDuration).SetEase(hoverGlowEase);
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
        // Quick way to prevent errors in case the crate is already being destroyed when this is called.
        if (crateRigidbody == null)
        {
            return;
        }

        this.grabPoint = null;

        crateRigidbody.linearDamping = defaultLinearDamping;
        crateRigidbody.useGravity = true;
    }
}
