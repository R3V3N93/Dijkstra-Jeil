using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "GManagerSO", menuName = "SO/GManagerSO")]
public class InputSO : ScriptableObject, InputSystem.IUIActions
{
    public bool enabled = true;
    
    public event Action eventClick;
    public event Action eventRightClick;
    public event Action eventDelete;
    public event Action eventCancel;
    
    public Vector2 mousePosition { get; private set; }
    public Vector2 mouseDelta { get; private set; }
    public Vector2 scroll { get; private set; }
    public bool middleClicked { get; private set; }
    public bool ctrl { get; private set; }

    private InputSystem pinput;

    public void OnEnable()
    {
        if (pinput == null)
        {
            pinput = new InputSystem();
            pinput.UI.SetCallbacks(this);
        }
        
        pinput.UI.Enable();
    }

    public void OnDisable()
    {
        enabled = true;
        pinput.UI.Disable();
    }

    public void OnDestroy()
    {
        enabled = true;
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        if(!enabled) return;
        mousePosition = context.ReadValue<Vector2>();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if(!enabled) return;
        if(context.canceled) eventClick?.Invoke();
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if(!enabled) return;
        if(context.canceled) eventRightClick?.Invoke();
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        if(!enabled) return;
        if(context.performed)    middleClicked = true;
        else                     middleClicked = false;
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        if(!enabled) return;
        scroll = context.ReadValue<Vector2>();
    }

    public void OnMouseDelta(InputAction.CallbackContext context)
    {
        if(!enabled) return;
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnDelete(InputAction.CallbackContext context)
    {
        if(!enabled) return;
        if(context.canceled) eventDelete?.Invoke();
    }

    public void OnCtrl(InputAction.CallbackContext context)
    {
        if(!enabled) return;
        if(context.performed)    ctrl = true;
        else                     ctrl = false;
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if(context.canceled) eventCancel?.Invoke();
    }
}


