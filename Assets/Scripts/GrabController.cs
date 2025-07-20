using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabController : MonoBehaviour
{
    [SerializeField] private UserInputListener userInputListener;
    [SerializeField] private Camera gameCamera;
    [SerializeField] private float maxGrabDistance = 100f;
    [SerializeField] private Transform grabPoint;
    [SerializeField] private CanvasGroup reticleTail;
    [SerializeField] private float reticleFadeDuration = 0.2f;
    [SerializeField] private Ease reticleFadeEase = Ease.OutSine;

    private LayerMask grabbableLayerMask;
    private Vector2 MousePos => Mouse.current.position.value;

    private IGrabbable heldGrabbable;
    private IGrabbable hoveringGrabbable;
    private Transform cameraTransform;

    void Awake()
    {
        cameraTransform = gameCamera.transform;
        grabbableLayerMask = LayerMask.GetMask("Grabbable");
        reticleTail.alpha = 0f;
    }

    void OnEnable()
    {
        userInputListener.OnGrab += TryGrab;
        userInputListener.OnRelease += TryRelease;
        userInputListener.OnLook += Look;
    }

    void OnDisable()
    {
        userInputListener.OnGrab -= TryGrab;
        userInputListener.OnRelease -= TryRelease;
        userInputListener.OnLook -= Look;
    }

    private void TryGrab()
    {
        if (IsPointingToGrabbable(out var grabbable))
        {
            heldGrabbable = grabbable;
            grabbable.Grab(grabPoint);
        }
    }

    private void TryRelease()
    {
        if (heldGrabbable != null)
        {
            heldGrabbable.Release();
            heldGrabbable = null;
        }
    }

    private void Look(Vector2 delta)
    {
        if (heldGrabbable != null)
        {
            return;
        }

        if (IsPointingToGrabbable(out var newGrabbable))
        {
            if (newGrabbable != hoveringGrabbable)
            {
                hoveringGrabbable?.EndHover();
                hoveringGrabbable = newGrabbable;
                hoveringGrabbable.BeginHover();
                reticleTail.DOKill();   
                reticleTail.DOFade(1f, reticleFadeDuration).SetEase(reticleFadeEase);
            }
        }
        else if (hoveringGrabbable != null)
        {
            hoveringGrabbable.EndHover();
            hoveringGrabbable = null;
            reticleTail.DOKill();
            reticleTail.DOFade(0f, reticleFadeDuration).SetEase(reticleFadeEase);
        }
    }

    private bool IsPointingToGrabbable(out IGrabbable grabbable)
    {
        grabbable = null;

        if (!Physics.Raycast(cameraTransform.position, cameraTransform.forward, out var hitInfo, maxGrabDistance, grabbableLayerMask))
        {
            return false;
        }

        return hitInfo.transform.TryGetComponent(out grabbable);
    }
}
