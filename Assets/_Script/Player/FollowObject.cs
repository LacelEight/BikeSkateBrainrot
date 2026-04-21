using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target not set for FollowObject script.");
        }

        offset = transform.position - target.position;
    }
    private void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}
