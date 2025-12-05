using System;
using UnityEngine;

public enum PainterMode
{
    Paint,
    Shovel,
    Bucket,
    Object,
    Destruction
}

[RequireComponent(typeof(PaintComponent))]
[RequireComponent(typeof(ShovelComponent))]
[RequireComponent(typeof(BucketComponent))]
[RequireComponent(typeof(ObjectPoserComponent))]
public class Painter : MonoBehaviour
{
    private PaintComponent paintComponent;
    private ObjectPoserComponent poserComponent;
    private ShovelComponent shovelComponent;
    private BucketComponent bucketComponent;
    private DestructionComponent destructionComponent;
    public PainterMode currentMode { get; private set; } = PainterMode.Paint;
    [SerializeField] private InputHandler inputHandler;
    private bool isPainting;
    private Vector2 mousePos;
    [SerializeField] private float brushSize = 1;

    private void Awake()
    {
        paintComponent = GetComponent<PaintComponent>();
        shovelComponent = GetComponent<ShovelComponent>();
        bucketComponent = GetComponent<BucketComponent>();
        poserComponent = GetComponent<ObjectPoserComponent>();
        destructionComponent = GetComponent<DestructionComponent>();
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
        mousePos = position;
    }

    private void Update()
    {
        if (isPainting)
        {
            if (currentMode == PainterMode.Shovel)
            {
                shovelComponent.Add(mousePos, brushSize);
                poserComponent.Remove(mousePos, brushSize);
            }
            else if (currentMode == PainterMode.Paint)
            {
                bucketComponent.Remove(mousePos, brushSize);
                if (!inputHandler.isShiftPressed)
                {
                    paintComponent.Add(mousePos, brushSize);
                }
                else
                {
                    paintComponent.Up(mousePos, brushSize);
                }
            }
            else if (currentMode == PainterMode.Bucket)
            {
                bucketComponent.Remove(mousePos, brushSize);
            }
            else if (currentMode == PainterMode.Object)
            {
                poserComponent.Add(mousePos, brushSize);
            }
            else if (currentMode == PainterMode.Destruction)
            {
                destructionComponent.Add(mousePos, brushSize);
            }
        }
    }

    private void OnMouseClickReleased()
    {
        isPainting = false;
    }

    private void OnMouseClickPressed()
    {
        isPainting = true;
    }

    private void OnDisable()
    {
        inputHandler.mouseClickPressed -= OnMouseClickPressed;
        inputHandler.mouseMoved -= OnMouseMoved;
        inputHandler.mouseClickReleased -= OnMouseClickReleased;
    }
}