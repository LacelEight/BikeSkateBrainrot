using System;
using System.Collections.Generic;
using Player.States;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public List<PlayerState> playerStates;
    public PlayerInputHandler InputHandler;

    public Animator Animator;
    [HideInInspector]
    public Transform Transform;
    public Transform PlayerBottom;
    public Rigidbody Rb;

    public BaseFootIK FootIk;

    [Header("Bike")]
    public BikeDriveVisual BikeDriveVisual;

    /// <summary>Tốc độ tuyến tính hiện tại khi đi xe (m/s); dùng cho visual và coast khi idle.</summary>
    public float BikeLinearSpeed;

    #region References
    public bool useCinemachine = false;


    private PlayerStateMachine stateMachine;

    private List<BikeArea> bikeAreas = new List<BikeArea>();

    [Header("References")]
    [SerializeField] private Transform cameraCinemachine;
    [SerializeField] private Transform headPoint;
    [SerializeField] private CamCtl camCtl;
    #endregion

    private Vector3 moveInput;
    private void Awake()
    {
        Transform = this.transform;
        if (!Rb) Rb = GetComponent<Rigidbody>();
        stateMachine = new PlayerStateMachine();
        InitStates();
    }

    private void OnEnable()
    {
        BikeArea.OnPlayerEnter += OnBikeAreaEnter;
        BikeArea.OnPlayerExit += OnBikeAreaExit;
    }

    private void OnDisable()
    {
        BikeArea.OnPlayerEnter -= OnBikeAreaEnter;
        BikeArea.OnPlayerExit -= OnBikeAreaExit;
    }
    private void Update()
    {
        moveInput = InputHandler.GetMoveInput();
        stateMachine.GetCurrentState().Update();
    }

    private void FixedUpdate()
    {
        camCtl.RotateCamera();
        stateMachine.GetCurrentState().PhysicsUpdate();
    }

    private void Start()
    {
        stateMachine.InitializeState(PlayerStateEnum.Idle);
    }

    private void InitStates()
    {
        foreach (var state in playerStates)
        {
            var stateIns = Instantiate(state);
            stateIns.Init(this, stateMachine);
            stateMachine.AddState(stateIns);
        }
    }

    public Vector3 GetMovementDirection(Vector2 moveInput)
    {
        if (moveInput.magnitude == 0)
            return Vector3.zero;

        Vector3 cameraForward = GetCameraForwardVector();
        Vector3 cameraRight = GetCameraRightVector();
        cameraRight.y = 0;

        //Debug.Log($"Camera Forward: {cameraForward}, Camera Right: {cameraRight}");
        Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;
        //Debug.Log($"Calculated Move Direction: {moveDirection} from Input: {moveInput}");
        return moveDirection;
    }

    private Vector3 GetCameraForwardVector()
    {
        Vector3 forward = useCinemachine
            ? headPoint.position - cameraCinemachine.position
            : camCtl.Target.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetCameraRightVector()
    {
        Vector3 right = useCinemachine
            ? Vector3.Cross(Vector3.up, headPoint.position - cameraCinemachine.position)
            : camCtl.Target.right;
        right.y = 0;
        return right.normalized;
    }

    private void OnBikeAreaEnter(BikeArea bikeArea)
    {
        bikeAreas.Add(bikeArea);
        FootIk.ActiveIk();
        stateMachine.SwitchState(moveInput.magnitude > 0.1f ? PlayerStateEnum.BikeRide : PlayerStateEnum.BikeIdle);
    }

    private void OnBikeAreaExit(BikeArea bikeArea)
    {
        bikeAreas.Remove(bikeArea);
        if (bikeAreas.Count == 0)
        {
            FootIk.DeactiveIk();
            stateMachine.SwitchState(moveInput.magnitude > 0.1f ? PlayerStateEnum.Walk : PlayerStateEnum.Idle);
        }
    }
}
