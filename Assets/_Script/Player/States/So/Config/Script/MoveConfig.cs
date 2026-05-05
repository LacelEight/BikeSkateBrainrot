using UnityEngine;

[CreateAssetMenu(fileName = "MoveConfig", menuName = "Player/Configs/MoveConfig")]
public class MoveConfig : ScriptableObject
{
    [Header("Move")]
    public float MoveSpeed = 5f;
    public float Acceleration = 2f;
    public float Deceleration = 2f;
    public float MaxSpeed = 10f;
    public float MinSpeed = 0f;
    public float MaxAcceleration = 10f;
    public float MinAcceleration = 0f;
    public float MaxDeceleration = 10f;

    [Header("Rotate")]
    public float RotateSpeed = 5f;
}
