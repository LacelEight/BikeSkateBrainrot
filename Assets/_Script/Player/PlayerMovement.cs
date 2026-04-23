using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    #region State Management
    private PlayerStateManager stateManager;
    private WalkState walkState;
    private BikeRideState bikeRideState;
    [SerializeField] private bool isRidingBike = false;
    #endregion

    #region Movement Settings
    [Header("Walk Settings")]
    public float WalkSpeed = 5f;
    public float WalkRotateSpeed = 5f;

    [Header("Bike Settings")]
    public float BikeSpeed = 10f;
    public float BikeRotateSpeed = 8f;
    public float BikeAcceleration = 2f;

    [Header("General")]
    public float JumpForce = 5f;
    public Rigidbody rb;
    #endregion

    #region Input Actions
    private InputActionAsset InputActions;
    private InputAction m_moveAction;
    private InputAction m_jumpAction;
    private Vector2 m_moveAmt;
    #endregion

    #region References
    public bool useCinemachine = false;

    [Header("References")]
    [SerializeField] private Transform cameraCinemachine;
    [SerializeField] private Transform headPoint;
    [SerializeField] private JumpCtl jumpCtl;
    [SerializeField] private CamCtl camCtl;
    #endregion

    private PlayerMovementContext playerContext;


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
        InputActions = Services.InputService.GetInputActions();
        m_moveAction = InputActions.FindAction("Move");
        m_jumpAction = InputActions.FindAction("Jump");

        if (!rb) rb = GetComponent<Rigidbody>();
        jumpCtl.Init(rb);

        // Initialize state management
        playerContext = new PlayerMovementContext(
            rb,
            jumpCtl,
            camCtl,
            headPoint,
            cameraCinemachine,
            useCinemachine);

        stateManager = new PlayerStateManager(playerContext);
        walkState = new WalkState(playerContext, WalkSpeed, WalkRotateSpeed);
        bikeRideState = new BikeRideState(playerContext, BikeSpeed, BikeRotateSpeed, BikeAcceleration);

        // Start with walk state
        stateManager.Initialize(walkState);
    }

    private void Update()
    {
#if UNITY_EDITOR
        m_moveAmt = m_moveAction.ReadValue<Vector2>();
#else
        m_moveAmt = Services.InputService.JoystickInput;
#endif

        // Handle state transitions
        HandleStateTransitions();

        // Pass input to current state
        bool jumpPressed = m_jumpAction.IsPressed();
        stateManager.HandleInput(m_moveAmt, jumpPressed);

    }

    private void FixedUpdate()
    {
        // Update camera rotation
        camCtl.RotateCamera();

        // Update movement via current state
        PlayerMovementContext context = GetCurrentContext();
        context.SetMoveInput(m_moveAmt);
        Debug.Log($"Current Move Input: {m_moveAmt}");
        stateManager.PhysicsUpdate();
    }

    /// <summary>
    /// Handle transitions between walk and bike states
    /// Override this method to add custom transition logic
    /// </summary>
    private void HandleStateTransitions()
    {
        // Example: Switch to bike when pressing a key (customize as needed)
        // if (Input.GetKeyDown(KeyCode.B))
        // {
        //     SwitchToBikeState();
        // }
        // if (Input.GetKeyDown(KeyCode.W))
        // {
        //     SwitchToWalkState();
        // }
    }

    #region Public State Control Methods
    public void SwitchToWalkState()
    {
        isRidingBike = false;
        stateManager.SwitchState(walkState);
    }

    public void SwitchToBikeState()
    {
        isRidingBike = true;
        stateManager.SwitchState(bikeRideState);
    }

    public bool IsRidingBike() => isRidingBike;

    public IPlayerMovementState GetCurrentMovementState() => stateManager.GetCurrentState();
    #endregion

    private PlayerMovementContext GetCurrentContext()
    {
        return playerContext;
    }
}
