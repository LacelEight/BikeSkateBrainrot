using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float WalkSpeed = 5f;
    public float JumpForce = 5f;
    public float RotateSpeed = 5f;
    public Vector2 CameraSpeed = new Vector2(20f, 5f);
    public Rigidbody rb;

    #region  Input actions
    public InputActionAsset InputActions;
    private InputAction m_moveAction;
    private InputAction m_jumpAction;
    private InputAction m_lookAction;
    private Vector2 m_moveAmt;
    private Vector2 m_lookAmt;
    #endregion
    public bool useCinemachine = false;
    [Header("References")]
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private Transform cameraPoint;
    [SerializeField]
    private Transform cameraCinemachine;
    [SerializeField]
    private Transform headPoint;
    [SerializeField]
    private JumpCtl jumpCtl;

    private Vector3 cameraRotation = Vector3.zero;

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
        jumpCtl.Init(rb);
    }

    private void Update()
    {
        m_moveAmt = m_moveAction.ReadValue<Vector2>();
        m_lookAmt = m_lookAction.ReadValue<Vector2>();

        if (m_jumpAction.IsPressed())
        {
            jumpCtl.Jump();
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
            Vector3 cameraRight = GetCameraRightVector2();
            cameraRight.y = 0;
            Vector3 moveDirection = (cameraForward * m_moveAmt.y + cameraRight * m_moveAmt.x).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, RotateSpeed * Time.fixedDeltaTime));
        }
    }

    private void RotateCamera()
    {
        float horizontalRotationAmount = m_lookAmt.x * CameraSpeed.x * Time.fixedDeltaTime;
        float verticalRotationAmount = m_lookAmt.y * CameraSpeed.y * Time.fixedDeltaTime;

        cameraRotation.x += verticalRotationAmount;
        cameraRotation.y += horizontalRotationAmount;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, -90f, 90f);
        cameraPoint.rotation = Quaternion.Euler(cameraRotation);
    }

    private void Moving()
    {
        //Debug.Log("moveAmt value: " + m_moveAmt);
        Vector3 cameraForward = GetCameraFowardVector2();
        Vector3 cameraRight = GetCameraRightVector2();
        cameraRight.y = 0;
        Vector3 movement = (cameraForward * m_moveAmt.y + cameraRight * m_moveAmt.x) * WalkSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    private Vector3 GetCameraFowardVector2()
    {
        Vector3 forward = useCinemachine ? headPoint.position - cameraCinemachine.position : cameraPoint.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetCameraRightVector2()
    {
        Vector3 right = useCinemachine ? Vector3.Cross(Vector3.up, headPoint.position - cameraCinemachine.position) : cameraPoint.right;
        right.y = 0;
        return right.normalized;
    }
}
