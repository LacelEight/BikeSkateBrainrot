using System;
using LacelSDK;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamCtl : MonoBehaviour
{
    private InputActionAsset inputActions;
    private InputAction m_SecondaryTouchAction;
    public void Init(InputActionAsset inputSystem)
    {
        inputActions = inputSystem;
        m_SecondaryTouchAction = inputActions.FindAction("SecondaryTouchContact");
        m_SecondaryTouchAction.started += StartZoom;
        m_SecondaryTouchAction.canceled += EndZoom;
    }

    private void OnDestroy()
    {
        if (m_SecondaryTouchAction != null)
        {
            m_SecondaryTouchAction.started -= StartZoom;
            m_SecondaryTouchAction.canceled -= EndZoom;
        }
    }

    private void StartZoom(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    private void EndZoom(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }
}
