using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "GManagerSO", menuName = "SO/GManagerSO")]
public class InputSO : ScriptableObject, InputSystem.IUIActions
{
    public event Action eventClick;
    public event Action eventRightClick;
    
    public Vector2 mousePosition { get; private set; }
    public Vector2 mouseDelta { get; private set; }
    public Vector2 scroll { get; private set; }
    public bool middleClicked { get; private set; }

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
        pinput.UI.Disable();
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if(context.canceled) eventClick?.Invoke();
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if(context.canceled) eventRightClick?.Invoke();
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        if(context.performed)    middleClicked = true;
        else                     middleClicked = false;
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        scroll = context.ReadValue<Vector2>();
    }

    public void OnMouseDelta(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }
}


