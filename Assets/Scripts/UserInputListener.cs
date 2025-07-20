using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInputListener : MonoBehaviour
{
    public static UserInputListener Instance { get; private set; }

    public static event Action OnGrab;
    public static event Action OnRelease;
    public static event Action<Vector2> OnLook;
    public static event Action<Vector2> OnMove;

    public Vector2 MoveValue { get; private set; }
    public Vector2 LookValue { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

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
            OnGrab?.Invoke();
        }
    }

    public void ReleaseInputReceived(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnRelease?.Invoke();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}
