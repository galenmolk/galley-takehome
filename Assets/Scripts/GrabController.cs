using UnityEngine;
using UnityEngine.InputSystem;

public class GrabController : MonoBehaviour
{
    [SerializeField] private UserInputListener userInputListener;
    [SerializeField] private Camera gameCamera;
    [SerializeField] private float maxGrabDistance = 100f;

    private LayerMask grabbableLayerMask;
    private Vector2 MousePos => Mouse.current.position.value;

    private IGrabbable heldGrabbable;
    private IGrabbable hoveringGrabbable;

    void Awake()
    {
        grabbableLayerMask = LayerMask.GetMask("Grabbable");
    }

    void OnEnable()
    {
        userInputListener.OnGrab += TryGrab;
        userInputListener.OnRelease += TryRelease;
        userInputListener.OnMouseMove += HandleMouseMove;
    }

    void OnDisable()
    {
        userInputListener.OnGrab -= TryGrab;
        userInputListener.OnRelease -= TryRelease;
        userInputListener.OnMouseMove -= HandleMouseMove;
    }

    private void TryGrab()
    {
        if (IsPointingToGrabbable(out var grabbable))
        {
            grabbable.Grab();
        }
    }

    private void TryRelease()
    {
        Debug.Log("TryRelease");
    }

    private void HandleMouseMove()
    {
        if (heldGrabbable != null)
        {
            Debug.Log($"Move Held Grabbable");
            return;
        }

        if (IsPointingToGrabbable(out var newGrabbable))
        {
            if (newGrabbable != hoveringGrabbable)
            {
                hoveringGrabbable?.EndHover();
                hoveringGrabbable = newGrabbable;
                hoveringGrabbable.BeginHover();
            }
        }
        else if (hoveringGrabbable != null)
        {
            hoveringGrabbable.EndHover();
            hoveringGrabbable = null;
        }
    }

    private bool IsPointingToGrabbable(out IGrabbable grabbable)
    {
        grabbable = null;
        var ray = gameCamera.ScreenPointToRay(MousePos);
        if (!Physics.Raycast(ray, out var hitInfo, maxGrabDistance, grabbableLayerMask))
        {
            return false;
        }

        grabbable = hitInfo.collider.GetComponent<IGrabbable>();
        return grabbable != null;
    }
}
