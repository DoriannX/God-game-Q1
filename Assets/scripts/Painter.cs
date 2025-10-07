using System;
using UnityEngine;
public enum PainterMode
{
    Paint,
    Shovel,
    Bucket
}
[RequireComponent(typeof(PaintComponent))]
[RequireComponent(typeof(ShovelComponent))]
[RequireComponent(typeof(BucketComponent))]
public class Painter : MonoBehaviour
{
    private PaintComponent paintComponent;
    private ShovelComponent shovelComponent;
    private BucketComponent bucketComponent;
    private PainterMode currentMode = PainterMode.Paint;
    [SerializeField] private InputHandler inputHandler;
    private bool isPainting;
    private Vector2 mousePos;
    [SerializeField] private float brushSize = 1;

    private void Awake()
    {
        paintComponent = GetComponent<PaintComponent>();
        shovelComponent = GetComponent<ShovelComponent>();
        bucketComponent = GetComponent<BucketComponent>();
    }
    
    public void SetBrushSize(float size)
    {
        brushSize = size;
    }
    
    public void SetMode(PainterMode mode)
    {
        currentMode = mode;
    }

    private void OnEnable()
    {
        inputHandler.mouseClickPressed += OnMouseClickPressed;
        inputHandler.mouseMoved += OnMouseMoved;
        inputHandler.mouseClickReleased += OnMouseClickReleased;
    }

    private void OnMouseMoved(Vector2 position)
    {
        mousePos = Camera.main != null ? Camera.main.ScreenToWorldPoint(position) : Vector3.zero;
    }

    private void Update()
    {
        if (isPainting)
        {
            if (currentMode == PainterMode.Shovel)
                shovelComponent.Add(mousePos, brushSize);
            else if( currentMode == PainterMode.Paint)
            {
                bucketComponent.Remove(mousePos, brushSize);
                paintComponent.Add(mousePos, brushSize);
            }
            else if (currentMode == PainterMode.Bucket)
                bucketComponent.Remove(mousePos, brushSize);
            
        }
    }

    private void OnMouseClickReleased()
    {
        isPainting = false;
    }
    
    private void OnMouseClickPressed()
    {
        isPainting = true;
        print("painting");
    }
    
    private void OnDisable()
    {
        inputHandler.mouseClickPressed -= OnMouseClickPressed;
        inputHandler.mouseMoved -= OnMouseMoved;
        inputHandler.mouseClickReleased -= OnMouseClickReleased;
    }
}
