using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public RectTransform joystickBase;
    public RectTransform joystickHandle;
    public RectTransform activationZone;

    public float maxRadius = 100f;

    private Vector2 input;
    private Vector2 basePosition;

    private void Start()
    {
        joystickBase.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Only activate if touch is within the activation zone
        if (!RectTransformUtility.RectangleContainsScreenPoint(
            activationZone, eventData.position, eventData.pressEventCamera))
            return;

        Services.InputService.SetJoystickPointerId(eventData.pointerId);

        // Convert screen position to canvas local position
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBase.parent as RectTransform, 
            eventData.position, 
            eventData.pressEventCamera, 
            out Vector2 localPosition))
        {
            // Set joystick base at first touch position
            joystickBase.anchoredPosition = localPosition;
            basePosition = localPosition;
            
            // Set handle to center of base (local position)
            joystickHandle.localPosition = Vector2.zero;
            
            joystickBase.gameObject.SetActive(true);
            input = Vector2.zero;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != Services.InputService.JoystickPointerId) 
            return;

        // Convert screen position to local position
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBase.parent as RectTransform, 
            eventData.position, 
            eventData.pressEventCamera, 
            out Vector2 localPosition))
        {
            // Calculate delta from base position
            Vector2 delta = localPosition - basePosition;
            
            // Clamp delta to maxRadius
            delta = Vector2.ClampMagnitude(delta, maxRadius);

            // Update handle position (local to base)
            joystickHandle.localPosition = delta;

            // Normalize input [-1, 1]
            input = delta / maxRadius;

            Services.InputService.SetJoystickInput(input);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != Services.InputService.JoystickPointerId) 
            return;

        Services.InputService.SetJoystickPointerId(-1);
        input = Vector2.zero;
        Services.InputService.SetJoystickInput(input);

        joystickBase.gameObject.SetActive(false);
    }
}
