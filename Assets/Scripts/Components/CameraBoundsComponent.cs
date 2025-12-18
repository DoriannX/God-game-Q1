using UnityEngine;

namespace Components
{
    [RequireComponent(typeof(Camera))]
    public class CameraBoundsComponent : MonoBehaviour
    {
        [SerializeField] private bool enableBounds = true;

        [SerializeField] private float minX = -50f;
        [SerializeField] private float maxX = 50f;
        [SerializeField] private float minZ = -50f;
        [SerializeField] private float maxZ = 50f;

        private float groundY = 0f;
        private Camera cam;

        private void Awake()
        {
            cam = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (!enableBounds) return;

            Plane groundPlane = new Plane(Vector3.up, new Vector3(0, groundY, 0));

            Vector3 limitOfMap = Vector3.zero;

            Vector3[] screenPoints =
            {
                new Vector2(0, 0),
                new Vector2(Screen.width, 0),
                new Vector2(0, Screen.height),
                new Vector2(Screen.width, Screen.height)
            };

            foreach (var screenPoint in screenPoints)
            {
                Ray ray = cam.ScreenPointToRay(screenPoint);

                if (!groundPlane.Raycast(ray, out float distance))
                    continue;

                Vector3 groundPoint = ray.GetPoint(distance);

                if (groundPoint.x < minX)
                    limitOfMap.x = Mathf.Max(limitOfMap.x, minX - groundPoint.x);

                if (groundPoint.x > maxX)
                    limitOfMap.x = Mathf.Min(limitOfMap.x, maxX - groundPoint.x);

                if (groundPoint.z < minZ)
                    limitOfMap.z = Mathf.Max(limitOfMap.z, minZ - groundPoint.z);

                if (groundPoint.z > maxZ)
                    limitOfMap.z = Mathf.Min(limitOfMap.z, maxZ - groundPoint.z);
            }

            transform.position += limitOfMap;
        }
    }
}
