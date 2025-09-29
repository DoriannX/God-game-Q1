using System;
using UnityEngine;

[RequireComponent(typeof(PaintComponent))]
public class Painter : MonoBehaviour
{
    private PaintComponent paintComponent;
    [SerializeField] private InputHandler inputHandler;
    private bool isPainting;
    private Vector2 mousePos;
    [SerializeField] private float brushSize = 1;

    private void Awake()
    {
        paintComponent = GetComponent<PaintComponent>();
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
            paintComponent.Add(mousePos, brushSize);
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
