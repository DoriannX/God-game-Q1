using System;
using UnityEngine;

public enum PainterMode
{
    Paint,
    Up,
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
    [SerializeField] private BrushSizeManager brushSizeManager;

    public PainterMode currentMode { get; private set; } = PainterMode.Paint;
    [SerializeField] private InputHandler inputHandler;
    private bool isPainting;
    private Vector2 mousePos;

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
        if (inputHandler.isCtrlPressed)
        {
            brushSizeManager.ChangeBrushSize(size);
        }
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
        inputHandler.mouseScrollStarted += SetBrushSize;
    }

    private void OnMouseMoved(Vector2 position)
    {
        mousePos = position;
        TilemapManager.instance.SetMousePos(mousePos);
    }

    private void Update()
    {
        if (isPainting)
        {
            if (currentMode == PainterMode.Shovel)
            {
                shovelComponent.Shovel();
            }
            else if (currentMode == PainterMode.Paint)
            {
                paintComponent.Paint();
            }
            else if (currentMode == PainterMode.Bucket)
            {
                bucketComponent.Bucket();
            }
            else if (currentMode == PainterMode.Object)
            {
                poserComponent.Place();
            }
            else if (currentMode == PainterMode.Destruction)
            {
                print ("Destruction mode active");
                destructionComponent.Destroy();
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