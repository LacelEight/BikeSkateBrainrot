using System;
using UnityEngine;

namespace Player.States
{
    [CreateAssetMenu(fileName = "BikeIdleState", menuName = "Player/States/BikeIdle")]
    public class BikeIdleState : PlayerState
    {
        private Vector2 moveInput;
        public override void OnEnter()
        {
            
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
                stateMachine.SwitchState(PlayerStateEnum.BikeRide);
            } 

            if (manager.InputHandler.IsJumpPressing())
            {
                stateMachine.SwitchState(PlayerStateEnum.BikeJump);
            }
        }
    }
}
