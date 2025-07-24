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
}