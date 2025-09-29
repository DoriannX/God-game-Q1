using UnityEngine;

[RequireComponent(typeof(MovementComponent))]
[RequireComponent(typeof(ZoomComponent))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private InputHandler inputHandler;
    private MovementComponent movementComponent;
    private ZoomComponent zoomComponent;
    private bool mouseClicked;
    [SerializeField] private float speed;
    [SerializeField] private float zoomSpeed;

    private void Awake()
    {
        movementComponent = GetComponent<MovementComponent>();
        zoomComponent = GetComponent<ZoomComponent>();
    }
    
    private void OnEnable()
    {
        inputHandler.mouseDeltaMoved += OnMouseDeltaMoved;
        inputHandler.mouseRightClickPressed += OnMouseClickPressed;
        inputHandler.mouseRightClickReleased += OnMouseClickReleased;
        inputHandler.mouseMiddleClickPressed += OnMouseClickPressed;
        inputHandler.mouseMiddleClickReleased += OnMouseClickReleased;
        inputHandler.mouseScrollStarted += OnMouseScrollStarted;
    }

    private void OnMouseScrollStarted(float delta)
    {
        zoomComponent.Zoom(delta, zoomSpeed);
    }

    private void OnMouseClickReleased()
    {
        mouseClicked = false;
    }

    private void OnDisable()
    {
        inputHandler.mouseDeltaMoved -= OnMouseDeltaMoved;
        inputHandler.mouseRightClickPressed -= OnMouseClickPressed;
        inputHandler.mouseRightClickReleased -= OnMouseClickReleased;
        inputHandler.mouseMiddleClickPressed -= OnMouseClickPressed;
        inputHandler.mouseMiddleClickReleased -= OnMouseClickReleased;
        inputHandler.mouseScrollStarted -= OnMouseScrollStarted;
    }

    private void OnMouseClickPressed()
    {
        mouseClicked = true;
    }

    private void OnMouseDeltaMoved(Vector2 delta)
    {
        if (!mouseClicked)
        {
            return;
        }
        movementComponent.Move(-delta, speed * zoomComponent.GetZoom() / 10);
    }
}
