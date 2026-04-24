using Cysharp.Threading.Tasks;
using Player.States;
using UnityEngine;

public class JumpState : PlayerState
{
    public float JumpForce = 5f;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.5f;
    public float jumpDelay = 0.1f;
    public int maxJumpCount = 1;
    private bool isGrounded;
    private int crrJumpCount = 0;

    public override void OnEnter()
    {

    }

    public override void OnExit()
    {

    }

    public override void PhysicsUpdate()
    {

    }

    public override void Update()
    {
        isGrounded = Physics.Raycast(manager.PlayerBottom.position, Vector3.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(manager.PlayerBottom.position, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }

    public void Jump()
    {
        Debug.Log("Jump " + crrJumpCount + " times" + $"(isGrounded: {isGrounded})");
        if (!isGrounded) return;
        if (crrJumpCount >= maxJumpCount) return;

        AddFJumpForce();
    }

    private void AddFJumpForce()
    {
        ResetForce();
        manager.Rb.AddForceAtPosition(Vector3.up * JumpForce, manager.Transform.position, ForceMode.Impulse);
        crrJumpCount++;
        WaitForGrounded().Forget();
        Debug.Log("Jumped! Current jump count: " + crrJumpCount);
    }

    private void ResetForce()
    {
        Vector3 velocity = manager.Rb.linearVelocity;
        velocity.y = 0;
        manager.Rb.linearVelocity = velocity;
    }

    private async UniTask WaitForGrounded()
    {
        await UniTask.WaitForSeconds(jumpDelay);
        await UniTask.WaitUntil(() => isGrounded);
        crrJumpCount = 0;
        Debug.Log("Landed! Jump count reset.");
    }
}
