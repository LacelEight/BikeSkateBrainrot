using UnityEngine;

/// <summary>
/// Walking state - normal movement on foot
/// </summary>
public class WalkState : IPlayerMovementState
{
    private PlayerMovementContext context;
    private float walkSpeed = 5f;
    private float rotateSpeed = 5f;

    public WalkState(PlayerMovementContext ctx, float speed = 5f, float rotSpeed = 5f)
    {
        context = ctx;
        walkSpeed = speed;
        rotateSpeed = rotSpeed;
    }

    public void OnEnter()
    {
        // Play walk animation if needed
        Debug.Log("Entered Walk State");
    }

    public void OnExit()
    {
        // Clean up walk state if needed
    }

    public void HandleInput(Vector2 moveInput, bool jumpPressed)
    {
        if (jumpPressed && CanJump())
        {
            context.jumpCtl.Jump();
        }
    }

    public void PhysicsUpdate()
    {
        Vector2 moveInput = context.GetMoveInput();
        
        // Rotate towards movement direction
        RotatePlayer(moveInput);

        // Apply movement
        MovePlayer(moveInput);
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

    private void MovePlayer(Vector2 moveInput)
    {
        Vector3 movement = context.GetMovementDirection(moveInput) * walkSpeed * Time.fixedDeltaTime;
        context.rb.MovePosition(context.rb.position + movement);
    }

    public float GetSpeed() => walkSpeed;
    public float GetRotationSpeed() => rotateSpeed;
    public bool CanJump() => true;
}
