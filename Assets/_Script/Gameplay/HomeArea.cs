using UnityEngine;

public class HomeArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Services.InventoryService.AddTempToInventory();
        }
    }
}
