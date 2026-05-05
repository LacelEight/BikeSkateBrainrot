using System;
using UnityEngine;

namespace Player.States
{
    [CreateAssetMenu(fileName = "BikeIdleState", menuName = "Player/States/BikeIdle")]
    public class BikeIdleState : PlayerState
    {
        [SerializeField]
        private MoveConfig bikeMoveConfig;
        private Vector2 moveInput;
        public override void OnEnter()
        {
            //manager.Animator.SetTrigger("BikeIdle");
        }

        public override void OnExit()
        {
            
        }

        public override void PhysicsUpdate()
        {
            float dt = Time.fixedDeltaTime;
            float decel = bikeMoveConfig != null ? bikeMoveConfig.Deceleration : 8f;
            manager.BikeLinearSpeed = Mathf.MoveTowards(manager.BikeLinearSpeed, 0f, decel * dt);
            manager.BikeDriveVisual?.Step(manager.BikeLinearSpeed, dt);
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
