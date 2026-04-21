using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float WalkSpeed = 5f;
    public float JumpForce = 5f;
    public float RotateSpeed = 5f;
    public Rigidbody rb;

    public InputActionAsset InputActions;

    private InputAction m_moveAction;
    private InputAction m_jumpAction;
    private InputAction m_lookAction;

    private Vector2 m_moveAmt;
    private Vector2 m_lookAmt;

    [Header("References")]

    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private Transform cameraPoint;

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        m_moveAction = InputActions.FindAction("Move");
        m_jumpAction = InputActions.FindAction("Jump");
        m_lookAction = InputActions.FindAction("Look");

        if (!rb) rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        m_moveAmt = m_moveAction.ReadValue<Vector2>();
        m_lookAmt = m_lookAction.ReadValue<Vector2>();

        if (m_jumpAction.WasPressedThisFrame())
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        Moving();
        Rotating();
    }

    private void Rotating()
    {
        RotateCamera();
        RotatePlayer();
    }

    private void RotatePlayer()
    {
        if (m_moveAmt.magnitude > 0)
        {
            Vector3 cameraForward = GetCameraFowardVector2();
            Vector3 cameraRight = cameraPoint.right;
            cameraRight.y = 0;
            Vector3 moveDirection = (cameraForward * m_moveAmt.y + cameraRight * m_moveAmt.x).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, RotateSpeed * Time.fixedDeltaTime));
        }
    }

    private void RotateCamera()
    {
        float horizontalRotationAmount = m_lookAmt.x * RotateSpeed * Time.fixedDeltaTime;
        float verticalRotationAmount = m_lookAmt.y * RotateSpeed * Time.fixedDeltaTime;

        Vector3 currentEuler = cameraPoint.rotation.eulerAngles;
        float newX = currentEuler.x + verticalRotationAmount;
        float newY = currentEuler.y + horizontalRotationAmount;

        cameraPoint.rotation = Quaternion.Euler(newX, newY, 0f);
    }

    private void Moving()
    {
        Debug.Log("moveAmt value: " + m_moveAmt);
        Vector3 cameraForward = GetCameraFowardVector2();
        Vector3 cameraRight = cameraPoint.right;
        cameraRight.y = 0;
        Vector3 movement = (cameraForward * m_moveAmt.y + cameraRight * m_moveAmt.x) * WalkSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    private void Jump()
    {
        rb.AddForceAtPosition(Vector3.up * JumpForce, transform.position, ForceMode.Impulse);
    }

    private Vector3 GetCameraFowardVector2(){
        Vector3 forward = cameraPoint.forward;
        forward.y = 0;
        return forward.normalized;
    }


}
