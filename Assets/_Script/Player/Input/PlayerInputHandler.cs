using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private InputActionAsset InputActions;
    private InputAction m_moveAction;
    private InputAction m_jumpAction;
    private Vector2 m_moveAmt;

    public Vector2 GetMoveInput()
    {
        return m_moveAmt;
    }

    public bool IsJumpPressing()
    {
        return m_jumpAction.IsPressed();
    }

    private void Awake()
    {
        InputActions = Services.InputService.GetInputActions();
        m_moveAction = InputActions.FindAction("Move");
        m_jumpAction = InputActions.FindAction("Jump");

        m_moveAction.performed += OnMovePerformed;
        m_moveAction.canceled += OnMoveCanceled;
    }

    private void OnDestroy()
    {
        m_moveAction.performed -= OnMovePerformed;
        m_moveAction.canceled -= OnMoveCanceled;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        m_moveAmt = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        m_moveAmt = Vector2.zero;
    }

    private void Update()
    {

    }

}
