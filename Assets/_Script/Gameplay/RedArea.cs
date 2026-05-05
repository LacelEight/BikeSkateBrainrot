using UnityEngine;

public class RedArea : MonoBehaviour
{
    
    public void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("Player"))
        {
            GameManager.OnPlayerDead?.Invoke();
        }
    }
}
