using UnityEngine;

namespace Components
{
    [RequireComponent(typeof(MovementComponent))]
    [RequireComponent(typeof(ZoomComponent))]
    [RequireComponent(typeof(CameraBoundsComponent))]
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
            if (!inputHandler.isCtrlPressed)
            {
                zoomComponent.Zoom(delta, zoomSpeed);
            }
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


            if (movementComponent == null)
            {
                Debug.LogError("Null reference detected: MovementComponent is not assigned.");
                return;
            }

            if (zoomComponent == null)
            {
                Debug.LogError("Null reference detected: ZoomComponent is not assigned.");
                return;
            }

            Vector3 direction = new Vector3(-delta.x, 0, -delta.y);
            movementComponent.Move(direction, speed * zoomComponent.GetZoom() / 10);
        }
    }
}

