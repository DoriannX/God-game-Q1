using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ZoomComponent : MonoBehaviour
{
    [SerializeField] private float minY = 5f;
    [SerializeField] private float maxY = 20f;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    public void Zoom(float delta, float zoomSpeed)
    {
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - delta * zoomSpeed, minY, maxY);
    }

    public float GetZoom()
    {
        return cam.orthographicSize;
    }
}
