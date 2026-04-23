using UnityEngine;

/// <summary>
/// Manages state transitions for player movement
/// </summary>
public class PlayerMovementStateManager
{
    private IPlayerMovementState currentState;
    private PlayerMovementContext context;

    public PlayerMovementStateManager(PlayerMovementContext ctx)
    {
        context = ctx;
    }

    public void Initialize(IPlayerMovementState initialState)
    {
        currentState = initialState;
        currentState?.OnEnter();
    }

    public void SwitchState(IPlayerMovementState newState)
    {
        if (currentState == newState)
            return;

        currentState?.OnExit();
        currentState = newState;
        currentState?.OnEnter();
    }

    public void HandleInput(Vector2 moveInput, bool jumpPressed)
    {
        currentState?.HandleInput(moveInput, jumpPressed);
    }

    public void PhysicsUpdate()
    {
        currentState?.PhysicsUpdate();
    }

    public IPlayerMovementState GetCurrentState() => currentState;
}
