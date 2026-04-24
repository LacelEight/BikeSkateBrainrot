using UnityEngine;

public class BaseGroundMotor : IMotor
{
    public virtual void Move(MotorContext ctx)
    {
        Vector3 movement = ctx.MoveDirection * ctx.MoveSpeed * Time.fixedDeltaTime;
        ctx.Rb.MovePosition(ctx.Rb.position + movement);
    }

    public virtual void Rotate(MotorContext ctx)
    {
        if (ctx.MoveInput.magnitude > 0)
        {
            Vector3 moveDirection = ctx.MoveDirection;
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            ctx.Rb.MoveRotation(Quaternion.Lerp(ctx.Rb.rotation, targetRotation, ctx.RotateSpeed * Time.fixedDeltaTime));
        }
    }
}
