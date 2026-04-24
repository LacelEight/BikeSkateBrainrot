using UnityEngine;

public interface IJump
{
    public void Jump(JumpControllerCtx ctx);

    public bool IsGrounded(JumpControllerCtx ctx);
}
