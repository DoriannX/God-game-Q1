using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
public class ZoomComponent : MonoBehaviour
{
    [SerializeField] private float minY = 5f;
    [SerializeField] private float maxY = 20f;
    private PixelPerfectCamera cam;

    private void Awake()
    {
        cam = GetComponent<PixelPerfectCamera>();
    }

    public void Zoom(float delta, float zoomSpeed)
    {
        cam.assetsPPU = (int)Mathf.Clamp(cam.assetsPPU + delta * zoomSpeed, minY, maxY);
    }

    public float GetZoom()
    {
        return cam.orthographicSize;
    }
}
