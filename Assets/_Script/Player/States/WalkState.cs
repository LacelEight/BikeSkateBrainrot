using System;
using UnityEngine;
namespace Player.States
{
    [CreateAssetMenu(fileName = "WalkState", menuName = "Player/States/Walk")]
    public class WalkState : PlayerState
    {
        [SerializeField]
        private MoveConfig config;
        private Vector2 moveInput;
        private IMotor motor;
        private MotorContext context;
        public override void OnEnter()
        {
            manager.Animator.SetTrigger("Walk");
            motor = new FootMotor();
            context = new MotorContext(config.MoveSpeed, config.RotateSpeed, manager.Rb);
        }

        public override void OnExit()
        {

        }

        public override void PhysicsUpdate()
        {
            if (moveInput.magnitude > 0.1f)
            {
                context.MoveDirection = manager.GetMovementDirection(moveInput);
                Debug.Log("Context Direction: " + context.MoveDirection);
                context.MoveInput = moveInput;
                motor.Move(context);
                motor.Rotate(context);
            }
        }

        public override void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            moveInput = manager.InputHandler.GetMoveInput();
            // Process move input to control player movement
            if (moveInput.magnitude <= 0.1f)
            {
                // If input is negligible, switch to Idle state
                stateMachine.SwitchState(PlayerStateEnum.Idle);
            }

            if (manager.InputHandler.IsJumpPressing())
            {
                stateMachine.SwitchState(PlayerStateEnum.Jump);
            }
        }
    }
}

