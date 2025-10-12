using System;
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
    private Selector selector;

    private void Awake()
    {
        selector = GetComponentInChildren<Selector>();
    }

    private void Start()
    {
        brushButton.onClick.AddListener(() =>
        {
            painter.SetMode(PainterMode.Paint);
            selector.Select(brushButton);
        });
        shovelButton.onClick.AddListener(() =>
        {
            painter.SetMode(PainterMode.Shovel);
            selector.Select(shovelButton);
        });
        bucketButton.onClick.AddListener(() =>
        {
            painter.SetMode(PainterMode.Bucket);
            selector.Select(bucketButton);
        });
        objectButton.onClick.AddListener(() =>
        {
            painter.SetMode(PainterMode.Object);
            selector.Select(objectButton);
        });
        destructionButton.onClick.AddListener(() =>
        {
            painter.SetMode(PainterMode.Destruction);
            selector.Select(destructionButton);
        });
        brushButton.onClick?.Invoke();
    }
}
