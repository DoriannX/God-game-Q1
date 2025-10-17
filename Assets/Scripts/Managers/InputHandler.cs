using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public event Action<Vector2> mouseDeltaMoved;
    public event Action<Vector2> mouseMoved;
    public event Action mouseStopped;

    public event Action mouseRightClickPressed;
    public event Action mouseRightClickReleased;
    
    public event Action mouseClickPressed;
    public event Action mouseClickReleased;
    
    public event Action mouseMiddleClickPressed;
    public event Action mouseMiddleClickReleased;
    
    public event Action<float> mouseScrollStarted;
    public event Action mouseScrollStopped;
    
    public event Action pausePressed;
    
    public void HandleMouseDeltaMoveInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            mouseDeltaMoved?.Invoke(ctx.ReadValue<Vector2>());
        }
        else if (ctx.canceled)
        {
            mouseStopped?.Invoke();
        }
    }
    
    public void HandleMouseMoveInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            mouseMoved?.Invoke(ctx.ReadValue<Vector2>());
        }
        else if (ctx.canceled)
        {
            mouseStopped?.Invoke();
        }
    }
    
    public void HandleMouseClickInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !EventSystem.current.IsPointerOverGameObject())
        {
            mouseClickPressed?.Invoke();
        }
        else if (ctx.canceled)
        {
            mouseClickReleased?.Invoke();
        }
    }
    
    public void HandleMouseRightClickInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !EventSystem.current.IsPointerOverGameObject())
        {
            mouseRightClickPressed?.Invoke();
        }
        else if (ctx.canceled)
        {
            mouseRightClickReleased?.Invoke();
        }
    }
    
    public void HandleMouseMiddleClickInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !EventSystem.current.IsPointerOverGameObject())
        {
            mouseMiddleClickPressed?.Invoke();
        }
        else if (ctx.canceled)
        {
            mouseMiddleClickReleased?.Invoke();
        }
    }
    
    public void HandleMouseScrollInput(InputAction.CallbackContext ctx)
    {
        float scrollValue = ctx.ReadValue<float>();
        if (ctx.performed)
        {
            mouseScrollStarted?.Invoke(scrollValue);
        }else if (ctx.canceled)
        {
            mouseScrollStopped?.Invoke();
        }
    }
    
    public void HandlePauseInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            pausePressed?.Invoke();
        }
    }
}
