using UnityEngine;

public class BikeMotor : BaseGroundMotor
{
    private float _currentSpeed;

    public override void Move(MotorContext ctx)
    {
        if (ctx.Rb == null)
            return;

        float dt = Time.fixedDeltaTime;
        float maxSpeed = ctx.MaxSpeed > 0f ? ctx.MaxSpeed : ctx.MoveSpeed;
        float inputMag = ctx.MoveInput.magnitude;
        float targetSpeed = inputMag > 0.01f ? Mathf.Clamp01(inputMag) * maxSpeed : 0f;

        if (ctx.Acceleration <= 0f)
            _currentSpeed = targetSpeed;
        else if (targetSpeed > _currentSpeed)
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, ctx.Acceleration * dt);
        else if (ctx.Deceleration <= 0f)
            _currentSpeed = targetSpeed;
        else
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, ctx.Deceleration * dt);

        ctx.CurrentSpeed = _currentSpeed;

        Vector3 dir = ctx.MoveDirection.sqrMagnitude > 0.0001f ? ctx.MoveDirection.normalized : Vector3.zero;
        Vector3 movement = dir * (_currentSpeed * dt);
        ctx.Rb.MovePosition(ctx.Rb.position + movement);
    }

    public override void Rotate(MotorContext ctx)
    {
        if (ctx.Rb == null)
            return;

        if (ctx.MoveInput.magnitude <= 0f && _currentSpeed <= 0.01f)
            return;

        base.Rotate(ctx);

        float signedAngle = Vector3.SignedAngle(
            ctx.MoveDirection,
            ctx.Rb.transform.forward,
            Vector3.up
        );

        ctx.CurrentRotate = signedAngle;

        Debug.Log("Signed Angle: " + signedAngle);
    }
}
