using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    public void Move(Vector2 direction, float speed)
    {
        transform.position += (Vector3)(direction * speed);
    }
}