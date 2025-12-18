using UnityEngine;

namespace Components
{
    public class CameraBoundsComponent : MonoBehaviour
    {
        [SerializeField] private bool enableBounds = true;
        [SerializeField] private float minX = -50f;
        [SerializeField] private float maxX = 50f;
        [SerializeField] private float minZ = -50f;
        [SerializeField] private float maxZ = 50f;

        private void LateUpdate()
        {
            if (!enableBounds) return;

            Vector3 position = transform.position;
            
            // Clamp the camera position
            position.x = Mathf.Clamp(position.x, minX, maxX);
            position.z = Mathf.Clamp(position.z, minZ, maxZ);
            
            transform.position = position;
        }

        // Optional: Draw gizmos to visualize bounds in the editor
        private void OnDrawGizmos()
        {
            if (!enableBounds) return;

            Gizmos.color = Color.yellow;

            Vector3 center = new Vector3((minX + maxX) / 2f, transform.position.y, (minZ + maxZ) / 2f);
            Vector3 size = new Vector3(maxX - minX, 0.1f, maxZ - minZ);

            Gizmos.DrawWireCube(center, size);
        }
    }
}