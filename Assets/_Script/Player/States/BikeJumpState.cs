using System;
using UnityEngine;

namespace Player.States
{
    [CreateAssetMenu(fileName = "BikeJumpState", menuName = "Player/States/BikeJump")]
    public class BikeJumpState : PlayerState
    {
        [SerializeField]
        private JumpConfig config;
        [SerializeField]
        private MoveConfig moveConfig;
        private IJump jumpController;
        private JumpControllerCtx context;
        private Vector2 moveInput;
        private IMotor motor;
        private MotorContext moveContext;
        private bool isGrounded = false;
        public override void OnEnter()
        {
            manager.Animator.SetTrigger("BikeJump");

            motor = new BikeMotor();
            // Restore velocity từ state trước đó
            if (motor is BikeMotor bikeMotor)
            {
                bikeMotor.InitializeSpeed(manager.BikeLinearSpeed);
            }
            moveContext = new MotorContext(moveConfig.MoveSpeed, moveConfig.RotateSpeed, manager.Rb);
            moveContext.Acceleration = moveConfig.Acceleration;
            moveContext.Deceleration = moveConfig.Deceleration;
            moveContext.MaxSpeed = moveConfig.MaxSpeed > 0f ? moveConfig.MaxSpeed : moveConfig.MoveSpeed;

            jumpController = new BikeJump();
            context = new JumpControllerCtx(manager.PlayerBottom, config.GroundLayer, config.GroundCheckDistance,
            config.maxJumpCount, config.JumpForce, manager.Rb, config.JumpInterval, OnLand);
        }

        public override void OnExit()
        {

        }

        public override void PhysicsUpdate()
        {
            if (manager.InputHandler.IsJumpPressing())
            {
                jumpController.Jump(context);
            }

            if (moveInput.magnitude > 0.1f)
            {

                moveContext.MoveDirection = manager.GetMovementDirection(moveInput);
                moveContext.MoveInput = moveInput;
                motor.Move(moveContext);
                motor.Rotate(moveContext);
                manager.BikeLinearSpeed = moveContext.CurrentSpeed;
            }
        }

        public override void Update()
        {
            HandleInput();
            isGrounded = jumpController.IsGrounded(context);
        }

        private void HandleInput()
        {
            moveInput = manager.InputHandler.GetMoveInput();
        }

        private void OnLand()
        {
            Debug.LogError("Landed!");
            stateMachine.SwitchState(moveInput.magnitude > 0.1f ? PlayerStateEnum.BikeRide : PlayerStateEnum.BikeIdle);
        }
    }
}
