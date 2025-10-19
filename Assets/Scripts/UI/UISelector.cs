// Assets/Scripts/UI/UISelector.cs

using System;
using UnityEngine;
using UnityEngine.UI;

public class UISelector : MonoBehaviour
{
    [SerializeField] private Image leftArrow;
    [SerializeField] private Image rightArrow;

    private void Start()
    {
        Deselect();
    }

    public void Select(GameObject selectedObject)
    {
        if (selectedObject == null)
        {
            Deselect();
            return;
        }

        var selectedRect = selectedObject.GetComponent<RectTransform>();
        if (selectedRect == null || leftArrow == null || rightArrow == null)
        {
            Deselect();
            return;
        }

        gameObject.SetActive(true);

        // 0=BL, 1=TL, 2=TR, 3=BR in world space
        var worldCorners = new Vector3[4];
        selectedRect.GetWorldCorners(worldCorners);

        var selectedCanvas = selectedRect.GetComponentInParent<Canvas>();
        var selectedCam = GetCanvasCamera(selectedCanvas);

        // Place left arrow at top-left corner
        PlaceArrowAtCorner(
            arrow: leftArrow.rectTransform,
            arrowCanvas: leftArrow.canvas,
            targetCornerWorld: worldCorners[1], // TL
            selectedCam: selectedCam,
            offset: new Vector2(-leftArrow.rectTransform.rect.width * 0.5f,
                -leftArrow.rectTransform.rect.height * 0.5f));

        // Place right arrow at top-right corner
        PlaceArrowAtCorner(
            arrow: rightArrow.rectTransform,
            arrowCanvas: rightArrow.canvas,
            targetCornerWorld: worldCorners[2], // TR
            selectedCam: selectedCam,
            offset: new Vector2(+rightArrow.rectTransform.rect.width * 0.5f,
                -rightArrow.rectTransform.rect.height * 0.5f));
    }

    public void Deselect()
    {
        gameObject.SetActive(false);
    }

    private static void PlaceArrowAtCorner(RectTransform arrow, Canvas arrowCanvas, Vector3 targetCornerWorld,
        Camera selectedCam, Vector2 offset)
    {
        if (arrow == null) return;

        var arrowParent = arrow.parent as RectTransform;
        if (arrowParent == null) return;

        var arrowCam = GetCanvasCamera(arrowCanvas);

        // World -> Screen
        var screenPoint = RectTransformUtility.WorldToScreenPoint(selectedCam, targetCornerWorld);

        // Screen -> Parent local
        RectTransformUtility.ScreenPointToLocalPointInRectangle(arrowParent, screenPoint, arrowCam, out var localPoint);

        // Apply local offset based on arrow size in its parent space
        arrow.anchoredPosition = localPoint + offset;
    }

    private static Camera GetCanvasCamera(Canvas canvas)
    {
        if (canvas == null) return null;
        return canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
    }
}