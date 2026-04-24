using UnityEngine;

[CreateAssetMenu(fileName = "JumpConfig", menuName = "Player/Configs/JumpConfig")]
public class JumpConfig : ScriptableObject
{
    public float JumpForce = 5f;
    public LayerMask GroundLayer;
    public float GroundCheckDistance = 0.5f;
    public float JumpInterval = 0.1f;
    public int maxJumpCount = 1;
}
