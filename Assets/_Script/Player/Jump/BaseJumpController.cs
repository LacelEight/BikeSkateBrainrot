using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BaseJumpCtl : IJump
{
    protected bool isGrounded;
    private int crrJumpCount = 0;
    private Action OnLand;
    public virtual void Jump(JumpControllerCtx ctx)
    {
        Debug.Log("Jump " + crrJumpCount + " times" + $"(isGrounded: {isGrounded})");
        if (!isGrounded) return;
        if (crrJumpCount >= ctx.MaxJumpCount) return;
        AddJumpForce(ctx);
        OnLand = ctx.OnLand;
    }
    public virtual bool IsGrounded(JumpControllerCtx ctx)
    {
        isGrounded = Physics.Raycast(ctx.RayStartPos, Vector3.down, ctx.GroundCheckDistance, ctx.GroundLayer);
        Debug.DrawRay(ctx.RayStartPos, Vector3.down * ctx.GroundCheckDistance, isGrounded ? Color.green : Color.red);
        return isGrounded;
    }

    private void AddJumpForce(JumpControllerCtx ctx)
    {
        ResetForce(ctx);
        ctx.Rb.AddForceAtPosition(Vector3.up * ctx.JumpForce, ctx.Rb.transform.position, ForceMode.Impulse);
        crrJumpCount++;
        WaitForGrounded(ctx).Forget();
        //Debug.Log("Jumped! Current jump count: " + crrJumpCount);
    }

    private void ResetForce(JumpControllerCtx ctx)
    {
        Vector3 velocity = ctx.Rb.linearVelocity;
        velocity.y = 0;
        ctx.Rb.linearVelocity = velocity;
    }

    private async UniTask WaitForGrounded(JumpControllerCtx ctx)
    {
        await UniTask.WaitForSeconds(ctx.JumpInterval);
        await UniTask.WaitUntil(() => isGrounded);
        crrJumpCount = 0;
        OnLand?.Invoke();
        //Debug.Log("Landed! Jump count reset.");
    }
}
