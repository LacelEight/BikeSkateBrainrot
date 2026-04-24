using UnityEngine;

public class MotorContext
{
    public float MoveSpeed;
    public float RotateSpeed;
    public Vector3 MoveDirection;
    public Vector2 MoveInput;

    public Rigidbody Rb;

    public MotorContext()
    {

    }

    /// <summary>
    /// Need assign MoveDirection and MoveInput realtime;
    /// </summary>
    /// <param name="moveSpeed"></param>
    /// <param name="rotateSpeed"></param>
    /// <param name="rb"></param>
    public MotorContext(float moveSpeed, float rotateSpeed, Rigidbody rb)
    {
        MoveSpeed = moveSpeed;
        RotateSpeed = rotateSpeed;
        Rb = rb;
    }
}
