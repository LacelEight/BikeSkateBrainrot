using System;
using Cysharp.Threading.Tasks;
using Player.States;
using UnityEngine;

namespace Player.States
{
    [CreateAssetMenu(fileName = "JumpState", menuName = "Player/States/Jump")]
    public class JumpState : PlayerState
    {
        [SerializeField]
        private JumpConfig config;
        [SerializeField]
        private MoveConfig moveConfig;
        private bool isGrounded;
        private int crrJumpCount = 0;
        private IJump jumpController;
        private JumpControllerCtx context;
        private Vector2 moveInput;
        private IMotor motor;
        private MotorContext moveContext;

        public override void OnEnter()
        {
            manager.Animator.SetTrigger("Jump");
            
            jumpController = new FootJump();
            motor = new FootMotor();
            moveContext = new MotorContext(moveConfig.MoveSpeed, moveConfig.RotateSpeed, manager.Rb);

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
            }
        }

        public override void Update()
        {
            jumpController.IsGrounded(context);
            HandleInput();
        }

        private void HandleInput()
        {
            moveInput = manager.InputHandler.GetMoveInput();
        }

        private void OnLand()
        {
            stateMachine.SwitchState(moveInput.magnitude > 0.1f ? PlayerStateEnum.Walk : PlayerStateEnum.Idle);
        }
    }
}
