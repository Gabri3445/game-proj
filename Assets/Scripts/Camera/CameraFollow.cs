using UnityEngine;

[ExecuteInEditMode]
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("No target assigned");
            return;
        }

        transform.position = target.position + offset;
    }

    private void LateUpdate()
    {
        if (target) transform.position = target.position + offset;
    }
}