using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class CategoryItemsConfig
{
    public string categoryId;               
    public GameObject itemsBarPrefab;       
    public RectTransform targetToolbarRoot; 
}

public class ToolbarManager : MonoBehaviour
{
    [Header("Parents possibles")]
    [SerializeField] private RectTransform mainToolbarRoot;  
    [SerializeField] private RectTransform secondaryToolbarRoot; 

    [Header("Catégories")]
    [SerializeField] private CategoryItemsConfig[] categories;

    [Header("Références")]
    [SerializeField] private PaletteSelector paletteSelector;


    private GameObject currentItemsBar;
    private string currentCategoryId;

    public void OpenCategory(string categoryId)
    {
        
        if (currentItemsBar != null && currentCategoryId == categoryId)
        {
            Debug.Log($"ToolbarManager: fermeture de la catégorie '{categoryId}'");
            Destroy(currentItemsBar);
            currentItemsBar = null;
            currentCategoryId = null;
            return;
        }

        
        if (currentItemsBar != null)
        {

            Destroy(currentItemsBar);
            currentItemsBar = null;
        }

        CategoryItemsConfig config = GetConfigForCategory(categoryId);
        if (config == null)
        {
            Debug.LogWarning($"ToolbarManager: aucune config trouvée pour la catégorie '{categoryId}'");
            return;
        }

        RectTransform parent = config.targetToolbarRoot != null
            ? config.targetToolbarRoot
            : mainToolbarRoot;

        if (parent == null)
        {
            Debug.LogError("ToolbarManager: aucun parent (RectTransform) défini pour instancier la barre d'items.");
            return;
        }

        
        GameObject barInstance = Instantiate(config.itemsBarPrefab, parent);
        barInstance.transform.localScale = Vector3.one; 

        currentItemsBar = barInstance;
        currentCategoryId = categoryId;

        var content = barInstance.GetComponent<ItemsBarContent>();

        if (content != null && paletteSelector != null)
        {
            paletteSelector.ConnectDynamicButtons(content.buttons, content.mode);
        }
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