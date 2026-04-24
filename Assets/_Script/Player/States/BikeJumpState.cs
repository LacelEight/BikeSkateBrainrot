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
        public override void OnEnter()
        {
            jumpController = new BikeJump();
            context = new JumpControllerCtx(manager.PlayerBottom.position, config.GroundLayer, config.GroundCheckDistance,
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
            }
        }

        public override void Update()
        {
            HandleInput();
            jumpController.IsGrounded(context);
        }

        private void HandleInput()
        {
            moveInput = manager.InputHandler.GetMoveInput();
        }

        private void OnLand()
        {
            stateMachine.SwitchState(moveInput.magnitude > 0.1f ? PlayerStateEnum.BikeRide : PlayerStateEnum.BikeIdle);
        }
    }
}
