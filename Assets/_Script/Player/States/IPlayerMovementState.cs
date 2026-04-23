using UnityEngine;

/// <summary>
/// Defines the interface for player movement states (Walk, Bike, etc.)
/// </summary>
public interface IPlayerMovementState
{
    /// <summary>
    /// Called when entering this state
    /// </summary>
    void OnEnter();

    /// <summary>
    /// Called when exiting this state
    /// </summary>
    void OnExit();

    /// <summary>
    /// Called every frame to handle input and update state
    /// </summary>
    void HandleInput(Vector2 moveInput, bool jumpPressed);

    /// <summary>
    /// Called during FixedUpdate to apply physics
    /// </summary>
    void PhysicsUpdate();

    /// <summary>
    /// Returns the movement speed for this state
    /// </summary>
    float GetSpeed();

    /// <summary>
    /// Returns the rotation speed for this state
    /// </summary>
    float GetRotationSpeed();

    /// <summary>
    /// Returns if the player can jump in this state
    /// </summary>
    bool CanJump();
}
