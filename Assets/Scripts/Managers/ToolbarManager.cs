using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;


public class ToolbarManager : MonoBehaviour
{
    [Header("Parents possibles")]
    [SerializeField] private RectTransform mainToolbarRoot;  

    [Header("References")]
    [SerializeField] private Painter painter;
    [SerializeField] private ObjectPoserComponent poserComponent;
    [SerializeField] private DestructionComponent destructionComponent;
    [SerializeField] private PaintComponent paintComponent;
    [SerializeField] private List<Image> tabImages;
    [SerializeField] private Sprite tabSelectedImage;
    [SerializeField] private Sprite tabDeselectedImage;
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

        switch (itemBarIndex)
        {
            case 0:
                tabImages[0].sprite = tabSelectedImage;
                tabImages[1].sprite = tabDeselectedImage;
                tabImages[2].sprite = tabDeselectedImage;
                break;
                
            case 1:
                tabImages[0].sprite = tabDeselectedImage;
                tabImages[1].sprite = tabSelectedImage;
                tabImages[2].sprite = tabDeselectedImage;
                break;
            
            case 2:
                tabImages[0].sprite = tabDeselectedImage;
                tabImages[1].sprite = tabDeselectedImage;
                tabImages[2].sprite = tabSelectedImage;
                break;
        }
        currentItemsBarContent = itemsBarContents[itemBarIndex];
        currentItemsBarContent.gameObject.SetActive(true);
    }
}