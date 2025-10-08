using UnityEngine;
using UnityEngine.UI;

public class ToolSelector : MonoBehaviour
{
    [SerializeField] private Painter painter;
    [SerializeField] private Button brushButton;
    [SerializeField] private Button shovelButton;
    [SerializeField] private Button bucketButton;
    [SerializeField] private Button objectButton;
    [SerializeField] private Button destructionButton;
    [SerializeField] private Image selectedIndicator;
    private void Start()
    {
        brushButton.onClick.AddListener(() =>
        {
            painter.SetMode(PainterMode.Paint);
            selectedIndicator.transform.position = brushButton.transform.position;
        });
        shovelButton.onClick.AddListener(() =>
        {
            painter.SetMode(PainterMode.Shovel);
            selectedIndicator.transform.position = shovelButton.transform.position;
        });
        bucketButton.onClick.AddListener(() =>
        {
            painter.SetMode(PainterMode.Bucket);
            selectedIndicator.transform.position = bucketButton.transform.position;
        });
        objectButton.onClick.AddListener(() =>
        {
            painter.SetMode(PainterMode.Object);
            selectedIndicator.transform.position = objectButton.transform.position;
        });
        destructionButton.onClick.AddListener(() =>
        {
            painter.SetMode(PainterMode.Destruction);
            selectedIndicator.transform.position = destructionButton.transform.position;
        });
        brushButton.onClick?.Invoke();
    }
}
