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
    [NonSerialized] public bool isCtrlPressed = false;
    private bool isMouseOnUI;

    public void HandleMouseDeltaMoveInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isMouseOnUI)
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
        if (ctx.performed && !isMouseOnUI)
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
        if (ctx.performed && !isMouseOnUI)
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
        if (ctx.performed && !isMouseOnUI)
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
        if (ctx.performed && !isMouseOnUI)
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
        if (ctx.performed && !isMouseOnUI)
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
    
    public void HandleShiftInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            isCtrlPressed = true;
        }
        else if (ctx.canceled)
        {
            isCtrlPressed = false;
        }
    }
    
    public void HandleControlInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            isCtrlPressed = true;
        }
        else if (ctx.canceled)
        {
            isCtrlPressed = false;
        }
    }

    private void Update()
    {
        isMouseOnUI = EventSystem.current.IsPointerOverGameObject();
    }
}
