using System;
using Cysharp.Threading.Tasks;
using LacelSDK;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputHandlerService", menuName = "Services/InputHandlerService")]
public class InputHandlerService : LacelService, IService
{
    public InputHandlerConfig config;
    public async UniTaskVoid InitAsync()
    {

    }

    public InputHandlerConfig GetConfig()
    {
        return config;
    }

    public bool IsTouchOverUI()
    {
        if (Touchscreen.current == null) return false;
        var touch = Touchscreen.current.primaryTouch;

        if (!touch.press.isPressed)
            return false;

        return EventSystem.current.IsPointerOverGameObject(touch.touchId.ReadValue());
    }

    public InputActionAsset GetInputActions()
    {
        return config.inputActions;
    }
}
