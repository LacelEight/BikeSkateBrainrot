using UnityEngine;

namespace Player.States
{
    /// <summary>
    /// Defines the interface for all player states (Walk, Bike, etc.)
    /// </summary>
    public abstract class PlayerState : ScriptableObject
    {
        public PlayerStateEnum Name;
        protected PlayerStateMachine stateMachine;
        protected PlayerManager manager;
        /// <summary>
        /// Called when entering this state
        /// </summary>
        public abstract void OnEnter();

        /// <summary>
        /// Called when exiting this state
        /// </summary>
        public abstract void OnExit();

        /// <summary>
        /// Called every frame to handle input and update state
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Called during FixedUpdate to apply physics
        /// </summary>
        public abstract void PhysicsUpdate();

        public virtual void Init(PlayerManager manager, PlayerStateMachine stateMachine)
        {
            this.manager = manager;
            this.stateMachine = stateMachine;
        }
    }

}

