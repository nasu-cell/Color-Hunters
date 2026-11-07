using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    
    void LateUpdate()
    {
        transform.position = target.position + offset;
    }
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
