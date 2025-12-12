using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaletteSelector : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] private Painter painter;
    [SerializeField] private ObjectPoserComponent poserComponent;
    [SerializeField] private DestructionComponent destructionComponent;
    [SerializeField] private PaintComponent paintComponent;

    public void ConnectDynamicButtons(Selector selector, List<Button> buttons, PainterMode mode)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            Button btn = buttons[i];
            int index = i;

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                selector.Select(btn);
                selector.transform.parent = btn.transform.parent;
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
}