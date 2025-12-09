using UnityEngine;
using UnityEngine.UI;

public class CategoryButtonUI : MonoBehaviour
{
    [SerializeField] private string categoryId; 

    private Button button;
    private ToolbarManager toolbarManager;

    private void Awake()
    {
        button = GetComponent<Button>();
        toolbarManager = FindObjectOfType<ToolbarManager>();
        button.onClick.AddListener(OnClickCategory);
    }

    private void OnClickCategory()
    {
        if (toolbarManager == null) return;
        toolbarManager.OpenCategory(categoryId);
    }
}
