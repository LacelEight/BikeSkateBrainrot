using System;
using UnityEngine;

public class JumpControllerCtx
{
    public Vector3 RayStartPos;

    public LayerMask GroundLayer;

    public float GroundCheckDistance;

    public int MaxJumpCount;

    public float JumpForce;

    public float JumpInterval;

    public Rigidbody Rb;

    public Action OnLand;

    /// <summary>
    /// Jump context for controller
    /// </summary>
    /// <param name="rayStartPos"></param>
    /// <param name="groundLayer"></param>
    /// <param name="groundCheckDistance"></param>
    /// <param name="maxJumpCount"></param>
    /// <param name="jumpForce"></param>
    /// <param name="rb"></param>
    /// <param name="jumpInterval"></param>
    public JumpControllerCtx(Vector3 rayStartPos, LayerMask groundLayer, float groundCheckDistance,
    int maxJumpCount, float jumpForce, Rigidbody rb, float jumpInterval, Action onLand)
    {
        RayStartPos = rayStartPos;
        GroundLayer = groundLayer;
        GroundCheckDistance = groundCheckDistance;
        MaxJumpCount = maxJumpCount;
        JumpForce = jumpForce;
        JumpInterval = jumpInterval;
        Rb = rb;
        OnLand = onLand;
    }
}
