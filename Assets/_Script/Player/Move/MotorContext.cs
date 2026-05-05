using UnityEngine;

public class MotorContext
{
    public float MoveSpeed;
    public float RotateSpeed;
    public Vector3 MoveDirection;
    public Vector2 MoveInput;

    /// <summary>Giới hạn tốc độ khi có input đầy đủ. Nếu &lt;= 0 dùng <see cref="MoveSpeed"/>.</summary>
    public float MaxSpeed;
    public float Acceleration;
    public float Deceleration;

    /// <summary>Tốc độ hiện tại sau bước tích phân (BikeMotor ghi vào mỗi Move).</summary>
    public float CurrentSpeed;
    public float CurrentRotate;

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
