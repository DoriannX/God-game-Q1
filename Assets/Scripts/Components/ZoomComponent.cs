using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ZoomComponent : MonoBehaviour
{
    [SerializeField] private float minZ = 5f;
    [SerializeField] private float maxZ = 50f;

    [SerializeField]
    private float zoomPlaneDistance = 0f; // Distance of the zoom plane from camera (use 0 for ground plane at y=0)

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    public void Zoom(float delta, float zoomSpeed)
    {
        // Create a ray from camera through mouse position
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // Find the point on the plane we want to zoom towards
        // Using a horizontal plane at y = zoomPlaneDistance (typically ground level)
        Plane plane = new Plane(Vector3.up, new Vector3(0, zoomPlaneDistance, 0));

        if (plane.Raycast(ray, out float distance))
        {
            // Get the world point under the mouse
            Vector3 worldPoint = ray.GetPoint(distance);

            // Calculate zoom direction (from camera towards mouse point)
            Vector3 zoomDirection = (worldPoint - transform.position).normalized;

            // Calculate zoom distance
            float zoomAmount = delta * zoomSpeed;

            // Move camera along the zoom direction
            Vector3 newPosition = transform.position + zoomDirection * zoomAmount;

            if (newPosition.y < minZ || newPosition.y > maxZ)
            {
                return;
            }

            // Apply the new position
            transform.position = newPosition;
            ChunkManager.Instance.UpdateVisibleChunks();
        }
    }

    public float GetZoom()
    {
        return transform.position.y;
    }
}