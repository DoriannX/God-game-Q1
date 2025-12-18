using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ToolbarManager : MonoBehaviour
{
    [Header("Parents possibles")]
    [SerializeField] private RectTransform mainToolbarRoot;  

    [Header("References")]
    [SerializeField] private Painter painter;
    [SerializeField] private ObjectPoserComponent poserComponent;
    [SerializeField] private DestructionComponent destructionComponent;
    [SerializeField] private PaintComponent paintComponent;
    private List<ItemsBarContent> itemsBarContents = new();
    private ItemsBarContent currentItemsBarContent;

    
    [Header("Outils Fixes")]
    [SerializeField] private Selector selector;

    private void Awake()
    {
        GetComponentsInChildren(true, itemsBarContents);


    }


    private void Start()
    {
        foreach (ItemsBarContent itemsBarContent in itemsBarContents)
        {
            itemsBarContent.Init();
            foreach (var tilemapBtn in itemsBarContent.buttons)
            {
                tilemapBtn.Init();
                tilemapBtn.button.onClick.RemoveAllListeners();
                var btn = tilemapBtn;
                tilemapBtn.button.onClick.AddListener(() =>
                {
                    selector.Select(btn);
                    selector.transform.parent = btn.transform.parent;
                    painter.SetMode(btn.mode);

                    switch (btn.mode)
                    {
                        case PainterMode.Object:
                            poserComponent.SetCurrentObject(btn.index);
                            break;
                        case PainterMode.Destruction:
                            break;
                        case PainterMode.Paint:
                            paintComponent.SetCurrentTile(btn.index);
                            break;
                    }
                });
            }
        }
        
        /*foreach (var itemsConfig in categories)
        {
            var content = itemsConfig.itemsBar.GetComponent<ItemsBarContent>();

            if (content != null && paletteSelector != null)
            {
                paletteSelector.ConnectDynamicButtons(selector,  content.buttons, content.mode);
            }
        }

        Button button = categories[0].itemsBar.GetComponent<ItemsBarContent>().buttons[0];
        selector.Select(button);
        selector.transform.parent = button.transform.parent;*/
    }

    public void ToggleCategory(int itemBarIndex)
    {
        // Close current items bar if one is open
        if (currentItemsBarContent != null)
        {
            bool isSameCategory = itemsBarContents[itemBarIndex] == currentItemsBarContent;
            currentItemsBarContent.gameObject.SetActive(false);
            currentItemsBarContent = null;
            
            // If clicking the same category, just close and return
            if (isSameCategory)
            {
                return;
            }
        }
        
        currentItemsBarContent = itemsBarContents[itemBarIndex];
        currentItemsBarContent.gameObject.SetActive(true);
    }
}