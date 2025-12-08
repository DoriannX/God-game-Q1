using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaletteSelector : MonoBehaviour
{
    [SerializeField] private Selector selector;
    [SerializeField] private Button shovelButton;
    [SerializeField] private Button bucketButton;
    [SerializeField] private Button upButton;
    [SerializeField] private List<Button> objectButtons;
    [SerializeField] private List<Button> destructionButtons;
    [SerializeField] private List<Button> tileButtons;
    [SerializeField] private Painter painter;
    [SerializeField] private ObjectPoserComponent poserComponent;
    [SerializeField] private DestructionComponent destructionComponent;
    [SerializeField] private PaintComponent paintComponent;

    private void Awake()
    {
        
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
        upButton.onClick.AddListener(() =>
        {
            painter.SetMode(PainterMode.Up);
            selector.Select(upButton);
        });
        
        for (var i = 0; i < objectButtons.Count; i++)
        {
            Button button = objectButtons[i];
            int index = i;
            button.onClick.AddListener(() =>
            {
                painter.SetMode(PainterMode.Object);
                poserComponent.SetCurrentObject(index);
                selector.Select(button);
            });
        }
        for (var i = 0; i < destructionButtons.Count; i++)
        {
            Button button = destructionButtons[i];
            int index = i;
            button.onClick.AddListener(() =>
            {
                painter.SetMode(PainterMode.Destruction);
                destructionComponent.SetCurrentObject(index);
                selector.Select(button);
            });
        }
        for (var i = 0; i < tileButtons.Count; i++)
        {
            Button button = tileButtons[i];
            int index = i;
            button.onClick.AddListener(() =>
            {
                painter.SetMode(PainterMode.Paint);
                paintComponent.SetCurrentTile(index);
                selector.Select(button);
            });
        }
        tileButtons[0].onClick?.Invoke();
    }
}
