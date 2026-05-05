using UnityEngine;

public class BikeController : MonoBehaviour
{
    public GameObject BikeRenderer;

    private void Start()
    {
        BikeRenderer.SetActive(false);
    }

}
