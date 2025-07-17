using System;
using Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInputListener : MonoBehaviour, PlayerControls.IPlayerActions
{
    public Action OnGrab;
    public Action OnRelease;
    public Action OnMouseMove;

    private PlayerControls playerControls;

    void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.Player.SetCallbacks(this);
        }

        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }

    void PlayerControls.IPlayerActions.OnMove(InputAction.CallbackContext context)
    {

    }

    void PlayerControls.IPlayerActions.OnLook(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnMouseMove?.Invoke();
        }
    }

    void PlayerControls.IPlayerActions.OnGrab(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnGrab?.Invoke();
        }
    }

    void PlayerControls.IPlayerActions.OnRelease(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnRelease?.Invoke();
        }
    }
}
