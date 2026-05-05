using System;
using UnityEngine;

namespace Player.States
{
    [CreateAssetMenu(fileName = "IdleState", menuName = "Player/States/Idle")]
    public class IdleState : PlayerState
    {
        Vector2 moveInput;
        public override void OnEnter()
        {
            manager.Animator.SetTrigger("Idle");
        }

        public override void OnExit()
        {
            
        }

        public override void PhysicsUpdate()
        {
            
        }

        public override void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            moveInput = manager.InputHandler.GetMoveInput();

            if (moveInput.magnitude > 0.1f)
            {
                // If there is significant input, switch to Walk state
                stateMachine.SwitchState(PlayerStateEnum.Walk);
            }

            if (manager.InputHandler.IsJumpPressing())
            {
                // If jump is pressed, switch to Jump state
                stateMachine.SwitchState(PlayerStateEnum.Jump);
            }
        }
    }
}
