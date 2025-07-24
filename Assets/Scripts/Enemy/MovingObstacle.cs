using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    public GameObject obstacle;
    public Transform start;
    public Transform end;
    public float speed;
    private bool _swap;

    private void Update()
    {
        if (_swap)
        {
            obstacle.transform.position =
                Vector3.MoveTowards(obstacle.transform.position, end.position, Time.deltaTime * speed);
            if (obstacle.transform.position == end.position) _swap = false;
        }
        else
        {
            obstacle.transform.position =
                Vector3.MoveTowards(obstacle.transform.position, start.position, Time.deltaTime * speed);
            if (obstacle.transform.position == start.position) _swap = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.75f, 0.0f, 0.0f, 0.75f);

        // Convert the local coordinate values into world
        // coordinates for the matrix transformation.
        Gizmos.matrix = start.transform.localToWorldMatrix;
        Gizmos.DrawSphere(Vector3.zero, 0.2f);
        Gizmos.matrix = end.transform.localToWorldMatrix;
        Gizmos.DrawSphere(Vector3.zero, 0.2f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.75f, 0.0f, 0.0f, 0.75f);

        // Convert the local coordinate values into world
        // coordinates for the matrix transformation.
        Gizmos.matrix = start.transform.localToWorldMatrix;
        Gizmos.DrawSphere(Vector3.zero, 0.2f);
        Gizmos.matrix = end.transform.localToWorldMatrix;
        Gizmos.DrawSphere(Vector3.zero, 0.2f);
    }
}