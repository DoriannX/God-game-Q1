using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaletteSelector : MonoBehaviour
{
    [Header("Outils Fixes")]
    [SerializeField] private Selector selector;
    [SerializeField] private Button shovelButton;
    [SerializeField] private Button bucketButton;
    [SerializeField] private Button upButton;

    [Header("Components")]
    [SerializeField] private Painter painter;
    [SerializeField] private ObjectPoserComponent poserComponent;
    [SerializeField] private DestructionComponent destructionComponent;
    [SerializeField] private PaintComponent paintComponent;

    private void Awake()
    {
    }

    public void ConnectDynamicButtons(List<Button> buttons, PainterMode mode)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            Button btn = buttons[i];
            int index = i;

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                painter.SetMode(mode);

                switch (mode)
                {
                    case PainterMode.Object:
                        poserComponent.SetCurrentObject(index);
                        break;
                    case PainterMode.Destruction:
                        break;
                    case PainterMode.Paint:
                        paintComponent.SetCurrentTile(index);
                        break;
                }
            });
        }
    }

    private void SetStaticTool(PainterMode mode, Button btn)
    {
        painter.SetMode(mode);
        selector.Select(btn);
    }
}