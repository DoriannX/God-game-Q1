using UnityEngine;

namespace Components
{
    public class MovementComponent : MonoBehaviour
    {
        public void Move(Vector3 direction, float speed)
        {
            transform.position += direction * speed;
        }
    }
}