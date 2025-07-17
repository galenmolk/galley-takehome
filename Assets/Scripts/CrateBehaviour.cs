using UnityEngine;

public interface IGrabbable
{
    void Grab();
    void Release();
    void BeginHover();
    void EndHover();
}

public class CrateBehaviour : MonoBehaviour, IGrabbable
{
    void IGrabbable.BeginHover()
    {
        Debug.Log($"Begin Hover");
    }

    void IGrabbable.EndHover()
    {
        Debug.Log($"End Hover");
    }

    void IGrabbable.Grab()
    {
        Debug.Log($"Grab");
    }

    void IGrabbable.Release()
    {
       Debug.Log($"Release");
    }
}
