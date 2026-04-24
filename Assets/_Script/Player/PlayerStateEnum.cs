using UnityEngine;

namespace Player.States
{
    /// <summary>
    /// Enum to represent different player states for easy reference
    /// </summary>
    public enum PlayerStateEnum
    {
        Idle = 0,
        Walk = 1,
        Run = 2,
        Jump = 3,
        Fall = 4,
        BikeIdle = 5,
        BikeRide = 6,
        BikeJump = 7
    }
}
