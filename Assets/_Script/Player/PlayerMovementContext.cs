using UnityEngine;

/// <summary>
/// Context object that holds shared data and utilities for all movement states
/// </summary>
public class PlayerMovementContext
{
    public Rigidbody rb;
    public JumpCtl jumpCtl;
    public CamCtl camCtl;
    public Transform headPoint;
    public Transform cameraCinemachine;
    public bool useCinemachine;

    private Vector2 currentMoveInput;

    public PlayerMovementContext(
        Rigidbody rigidbody,
        JumpCtl jumpControl,
        CamCtl cameraControl,
        Transform head,
        Transform cinemachineCamera,
        bool useCinem)
    {
        rb = rigidbody;
        jumpCtl = jumpControl;
        camCtl = cameraControl;
        headPoint = head;
        cameraCinemachine = cinemachineCamera;
        useCinemachine = useCinem;
    }

    public void SetMoveInput(Vector2 moveInput)
    {
        currentMoveInput = moveInput;
    }

    public Vector2 GetMoveInput()
    {
        return currentMoveInput;
    }

    /// <summary>
    /// Calculates the movement direction relative to camera orientation
    /// </summary>
    public Vector3 GetMovementDirection(Vector2 moveInput)
    {
        if (moveInput.magnitude == 0)
            return Vector3.zero;

        Vector3 cameraForward = GetCameraForwardVector();
        Vector3 cameraRight = GetCameraRightVector();
        cameraRight.y = 0;

        Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;
        return moveDirection;
    }

    private Vector3 GetCameraForwardVector()
    {
        Vector3 forward = useCinemachine 
            ? headPoint.position - cameraCinemachine.position 
            : camCtl.Target.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetCameraRightVector()
    {
        Vector3 right = useCinemachine 
            ? Vector3.Cross(Vector3.up, headPoint.position - cameraCinemachine.position) 
            : camCtl.Target.right;
        right.y = 0;
        return right.normalized;
    }
}
