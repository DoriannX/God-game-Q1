using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CategoryItemsConfig
{
    public string categoryId;               
    public GameObject itemsBar;       
    public RectTransform targetToolbarRoot; 
}

public class ToolbarManager : MonoBehaviour
{
    [Header("Parents possibles")]
    [SerializeField] private RectTransform mainToolbarRoot;  

    [Header("Cat�gories")]
    [SerializeField] private CategoryItemsConfig[] categories;

    [Header("R�f�rences")]
    [SerializeField] private PaletteSelector paletteSelector;
    
    [Header("Outils Fixes")]
    [SerializeField] private Selector selector;


    private GameObject currentItemsBar;
    private string currentCategoryId;


    private void Start()
    {
        foreach (var itemsConfig in categories)
        {
            var content = itemsConfig.itemsBar.GetComponent<ItemsBarContent>();

            if (content != null && paletteSelector != null)
            {
                paletteSelector.ConnectDynamicButtons(selector,  content.buttons, content.mode);
            }
        }

        Button button = categories[0].itemsBar.GetComponent<ItemsBarContent>().buttons[0];
        selector.Select(button);
        selector.transform.parent = button.transform.parent;
    }

    public void OpenCategory(string categoryId)
    {
        // Close current items bar if one is open
        if (currentItemsBar != null)
        {
            bool isSameCategory = currentCategoryId == categoryId;
            currentItemsBar.SetActive(false);
            currentItemsBar = null;
            currentCategoryId = null;
            
            // If clicking the same category, just close and return
            if (isSameCategory)
            {
                return;
            }
        }

        CategoryItemsConfig config = GetConfigForCategory(categoryId);
        if (config == null)
        {
            Debug.LogWarning($"ToolbarManager: aucune config trouv�e pour la cat�gorie '{categoryId}'");
            return;
        }

        RectTransform parent = config.targetToolbarRoot != null
            ? config.targetToolbarRoot
            : mainToolbarRoot;

        if (parent == null)
        {
            Debug.LogError("ToolbarManager: aucun parent (RectTransform) d�fini pour instancier la barre d'items.");
            return;
        }
        
        currentItemsBar = config.itemsBar;
        currentItemsBar.SetActive(true);
        currentCategoryId = categoryId;
    }

    private CategoryItemsConfig GetConfigForCategory(string categoryId)
    {
        foreach (var c in categories)
        {
            if (c != null && c.categoryId == categoryId)
            {
                return c;
            }

        }
        return null;
    }
}