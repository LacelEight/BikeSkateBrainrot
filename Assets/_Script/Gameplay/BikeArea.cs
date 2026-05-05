using System;
using UnityEngine;

public class BikeArea : MonoBehaviour
{
    public static Action<BikeArea> OnPlayerEnter;
    public static Action<BikeArea> OnPlayerExit;
    public void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("Player"))
        {
            OnPlayerEnter?.Invoke(this);
        }
    }

    public void OnTriggerExit(Collider hit)
    {
        if (hit.CompareTag("Player"))
        {
            OnPlayerExit?.Invoke(this);
        }
    }
}
