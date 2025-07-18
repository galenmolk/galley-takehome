using UnityEngine;

public interface IGrabbable
{
    void Grab(Transform grabPoint);
    void Release();
    void BeginHover();
    void EndHover();
    void Drag(Vector3 velocity);
}

public class CrateBehaviour : MonoBehaviour, IGrabbable
{
    [SerializeField] private Rigidbody crateRigidbody;
    [SerializeField] private float dragLerpSpeed = 30f;

    private Transform grabPoint;

    public void Drag(Vector3 targetPoint)
    {
        
    }

    void FixedUpdate()
    {
        if (grabPoint == null)
        {
            return;
        }

        var newPosition = Vector3.Lerp(transform.position, grabPoint.position, Time.deltaTime * dragLerpSpeed);
        Debug.Log($"Dragging {newPosition}");
        crateRigidbody.MovePosition(newPosition);
    }

    void IGrabbable.BeginHover()
    {
        Debug.Log($"Begin Hover");
    }

    void IGrabbable.EndHover()
    {
        Debug.Log($"End Hover");
    }

    void IGrabbable.Grab(Transform grabPoint)
    {
        Debug.Log($"Grab");
        this.grabPoint = grabPoint;
        crateRigidbody.useGravity = false;
    }

    void IGrabbable.Release()
    {
        Debug.Log($"Release");
        this.grabPoint = null;
        crateRigidbody.useGravity = true;
    }
}
