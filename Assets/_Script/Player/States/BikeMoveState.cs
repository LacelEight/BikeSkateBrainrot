using System;
using UnityEngine;

namespace Player.States
{
    [CreateAssetMenu(fileName = "BikeMoveState", menuName = "Player/States/BikeMove")]
    public class BikeMoveState : PlayerState
    {
        [SerializeField]
        private MoveConfig config;
        private Vector2 moveInput;
        private IMotor motor;
        private MotorContext context;
        public override void OnEnter()
        {
            manager.Animator.SetTrigger("BikeRun");
            motor = new BikeMotor();
            context = new MotorContext(config.MoveSpeed, config.RotateSpeed, manager.Rb);
        }

        public override void OnExit()
        {
        }

        public override void PhysicsUpdate()
        {
            if (moveInput.magnitude > 0.1f)
            {
                context.MaxSpeed = config.MaxSpeed > 0f ? config.MaxSpeed : config.MoveSpeed;
                context.Acceleration = config.Acceleration;
                context.Deceleration = config.Deceleration;
                context.MoveDirection = manager.GetMovementDirection(moveInput);
                context.MoveInput = moveInput;
                motor.Move(context);
                motor.Rotate(context);
                manager.BikeLinearSpeed = context.CurrentSpeed;
                manager.BikeDriveVisual?.Step(manager.BikeLinearSpeed, Time.fixedDeltaTime);
            }
        }

        public override void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            moveInput = manager.InputHandler.GetMoveInput();

            if (moveInput.magnitude <= 0.1f)
            {
                stateMachine.SwitchState(PlayerStateEnum.BikeIdle);
            }

            if (manager.InputHandler.IsJumpPressing())
            {
                stateMachine.SwitchState(PlayerStateEnum.BikeJump);
            }
        }
    }
}
