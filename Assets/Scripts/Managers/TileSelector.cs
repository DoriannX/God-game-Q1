using UnityEngine;

public class TileSelector : MonoBehaviour
{
    [System.Serializable]
    public class TileData
    {
        public string tileName;
        public GameObject tilePrefab;
    }
    
    [Header("Tile Selection")]
    [SerializeField] private TileData[] availableTiles; // Liste des tiles disponibles
    [SerializeField] private int currentTileIndex = 0; // Index de la tile sélectionnée
    [SerializeField] private float scrollSensitivity = 1f; // Sensibilité du scroll
    
    [Header("UI Debug")]
    [SerializeField] private bool showTileInfo = true;
    
    private float scrollAccumulator = 0f;
    
    private void Start()
    {
        if (availableTiles == null || availableTiles.Length == 0)
        {
            Debug.LogWarning("No tiles available in TileSelector!");
        }
        else
        {
            // S'assurer que l'index est valide
            currentTileIndex = Mathf.Clamp(currentTileIndex, 0, availableTiles.Length - 1);
        }
    }
    
    private void Update()
    {
        // Détecter le scroll de la molette pour changer de tile
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if (scroll != 0f && availableTiles != null && availableTiles.Length > 0)
        {
            scrollAccumulator += scroll * scrollSensitivity;
            
            // Changer de tile quand l'accumulateur dépasse 0.1
            if (Mathf.Abs(scrollAccumulator) >= 0.1f)
            {
                if (scrollAccumulator > 0)
                {
                    // Scroll vers le haut : tile suivante
                    currentTileIndex++;
                    if (currentTileIndex >= availableTiles.Length)
                    {
                        currentTileIndex = 0; // Boucler au début
                    }
                }
                else
                {
                    // Scroll vers le bas : tile précédente
                    currentTileIndex--;
                    if (currentTileIndex < 0)
                    {
                        currentTileIndex = availableTiles.Length - 1; // Boucler à la fin
                    }
                }
                
                scrollAccumulator = 0f;
            }
        }
    }
    
    // Obtenir la tile actuellement sélectionnée
    public GameObject GetCurrentTilePrefab()
    {
        if (availableTiles == null || availableTiles.Length == 0)
        {
            return null;
        }
        
        currentTileIndex = Mathf.Clamp(currentTileIndex, 0, availableTiles.Length - 1);
        return availableTiles[currentTileIndex].tilePrefab;
    }
    
    // Obtenir le nom de la tile actuellement sélectionnée
    public string GetCurrentTileName()
    {
        if (availableTiles == null || availableTiles.Length == 0)
        {
            return "None";
        }
        
        currentTileIndex = Mathf.Clamp(currentTileIndex, 0, availableTiles.Length - 1);
        return availableTiles[currentTileIndex].tileName;
    }
    
    // Obtenir l'index de la tile actuelle
    public int GetCurrentTileIndex()
    {
        return currentTileIndex;
    }
    
    // Définir manuellement la tile sélectionnée
    public void SetCurrentTileIndex(int index)
    {
        if (availableTiles != null && availableTiles.Length > 0)
        {
            currentTileIndex = Mathf.Clamp(index, 0, availableTiles.Length - 1);
        }
    }
    
    // Obtenir le nombre total de tiles disponibles
    public int GetTileCount()
    {
        return availableTiles != null ? availableTiles.Length : 0;
    }
    
    // Afficher les informations de la tile sélectionnée
    private void OnGUI()
    {
        if (showTileInfo && availableTiles != null && availableTiles.Length > 0)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 14;
            style.alignment = TextAnchor.UpperLeft;
            
            float x = 10f;
            float y = 10f;
            float width = 300f;
            float lineHeight = 25f;
            
            GUI.Label(new Rect(x, y, width, lineHeight), $"Selected Tile: {GetCurrentTileName()}", style);
            GUI.Label(new Rect(x, y + lineHeight, width, lineHeight), $"Tile {currentTileIndex + 1}/{availableTiles.Length}", style);
            GUI.Label(new Rect(x, y + lineHeight * 2, width, lineHeight), "Scroll to change tile", style);
        }
    }
}

