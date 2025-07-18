using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInputListener : MonoBehaviour
{
    public event Action OnGrab;
    public event Action OnRelease;
    public event Action<Vector2> OnLook;
    public event Action<Vector2> OnMove;

    public Vector2 MoveValue { get; private set; }
    public Vector2 LookValue { get; private set; }

    public void MoveInputReceived(InputAction.CallbackContext context)
    {
        MoveValue = context.ReadValue<Vector2>();
    }

    public void LookInputReceived(InputAction.CallbackContext context)
    {
        LookValue = context.ReadValue<Vector2>();

        if (context.performed)
        {
            OnLook?.Invoke(LookValue);
        }
    }

    public void GrabInputReceived(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Grab");
            OnGrab?.Invoke();
        }
    }

    public void ReleaseInputReceived(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Release");
            OnRelease?.Invoke();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}
