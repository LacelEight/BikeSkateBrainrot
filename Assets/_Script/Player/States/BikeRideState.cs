using UnityEngine;

/// <summary>
/// Bike riding state - movement while riding a bike
/// </summary>
public class BikeRideState : IPlayerMovementState
{
    private PlayerMovementContext context;
    private float bikeSpeed = 10f;
    private float rotateSpeed = 8f;
    private float acceleration = 2f;
    private float currentSpeed = 0f;

    public BikeRideState(PlayerMovementContext ctx, float speed = 10f, float rotSpeed = 8f, float accel = 2f)
    {
        context = ctx;
        bikeSpeed = speed;
        rotateSpeed = rotSpeed;
        acceleration = accel;
    }

    public void OnEnter()
    {
        currentSpeed = 0f;
        Debug.Log("Entered Bike Ride State");
    }

    public void OnExit()
    {
        currentSpeed = 0f;
        // Clean up bike state if needed
    }

    public void HandleInput(Vector2 moveInput, bool jumpPressed)
    {
        // Bike doesn't jump typically, but you can customize this
        // if (jumpPressed && CanJump())
        // {
        //     context.jumpCtl.Jump();
        // }
    }

    public void PhysicsUpdate()
    {
        Vector2 moveInput = context.GetMoveInput();
        
        // Rotate towards movement direction
        RotatePlayer(moveInput);

        // Apply bike movement with acceleration
        MoveBike(moveInput);
    }

    private void RotatePlayer(Vector2 moveInput)
    {
        if (moveInput.magnitude > 0)
        {
            Vector3 moveDirection = context.GetMovementDirection(moveInput);
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            context.rb.MoveRotation(Quaternion.Lerp(context.rb.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime));
        }
    }

    private void MoveBike(Vector2 moveInput)
    {
        if (moveInput.magnitude > 0)
        {
            // Accelerate towards bike speed
            currentSpeed = Mathf.Lerp(currentSpeed, bikeSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            // Decelerate when no input
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, acceleration * Time.fixedDeltaTime);
        }

        Vector3 movement = context.GetMovementDirection(moveInput) * currentSpeed * Time.fixedDeltaTime;
        context.rb.MovePosition(context.rb.position + movement);
    }

    public float GetSpeed() => bikeSpeed;
    public float GetRotationSpeed() => rotateSpeed;
    public bool CanJump() => false; // Can't jump while on bike
}
