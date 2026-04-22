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

    private int joystickPointerId = -1;
    private Vector2 joystickInput = Vector2.zero;
    public Vector2 JoystickInput => joystickInput;
    public int JoystickPointerId => joystickPointerId;
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

        return IsPointerOverUI(touch.position.ReadValue());
    }

    public InputActionAsset GetInputActions()
    {
        return config.inputActions;
    }

    public bool IsPointerOverUI(Vector2 screenPos)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = screenPos;

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }

    public void SetJoystickPointerId(int pointerId)
    {
        joystickPointerId = pointerId;
    }

    public void SetJoystickInput(Vector2 input)
    {
        joystickInput = input;
    }
}
