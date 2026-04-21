using Cysharp.Threading.Tasks;
using UnityEngine;

public class JumpCtl : MonoBehaviour
{
    public float JumpForce = 5f;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.2f;
    public float jumpDelay = 0.15f;
    public Transform playerBottom;
    private Rigidbody rb;
    private Transform playerTransform;
    public int maxJumpCount = 1;
    private bool isGrounded;
    private int crrJumpCount = 0;
    private void Update()
    {
        if (!rb) return;
        isGrounded = Physics.Raycast(playerBottom.position, Vector3.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(playerBottom.position, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
        //Debug.LogError($"isGrounded: {isGrounded}, crrJumpCount: {crrJumpCount}");
    }

    public void Init(Rigidbody rb)
    {
        this.rb = rb;
        playerTransform = rb.transform;
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
        rb.AddForceAtPosition(Vector3.up * JumpForce, transform.position, ForceMode.Impulse);
        crrJumpCount++;
        WaitForGrounded().Forget();
        Debug.Log("Jumped! Current jump count: " + crrJumpCount);
    }

    private void ResetForce()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0;
        rb.linearVelocity = velocity;
    }

    private async UniTask WaitForGrounded()
    {
        await UniTask.WaitForSeconds(jumpDelay);
        await UniTask.WaitUntil(() => isGrounded);
        crrJumpCount = 0;
        Debug.Log("Landed! Jump count reset.");
    }
}
