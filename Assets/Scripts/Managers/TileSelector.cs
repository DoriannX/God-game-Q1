using System;
using UnityEngine;

public class TileSelector : MonoBehaviour
{
    [System.Serializable]
    public class TileData
    {
        public string tileName;
        public Sprite tileIcon;
        public GameObject tilePrefab;
    }

    [field: Header("Tile Selection")]
    [field: SerializeField]
    public TileData[] AvailableTiles { get; private set; }

    [field: SerializeField] public int CurrentTileIndex { get; private set; } = 0;
    [SerializeField] private float scrollSensitivity = 1f; // Sensibilité du scroll
    
    [Header("UI Debug")]
    [SerializeField] private bool showTileInfo = true;
    
    private float scrollAccumulator = 0f;
    
    public static TileSelector instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (AvailableTiles == null || AvailableTiles.Length == 0)
        {
            Debug.LogWarning("No tiles available in TileSelector!");
        }
        else
        {
            // S'assurer que l'index est valide
            CurrentTileIndex = Mathf.Clamp(CurrentTileIndex, 0, AvailableTiles.Length - 1);
        }
    }
    
    private void Update()
    {
        // Détecter le scroll de la molette pour changer de tile
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if (scroll != 0f && AvailableTiles != null && AvailableTiles.Length > 0)
        {
            scrollAccumulator += scroll * scrollSensitivity;
            
            // Changer de tile quand l'accumulateur dépasse 0.1
            if (Mathf.Abs(scrollAccumulator) >= 0.1f)
            {
                if (scrollAccumulator > 0)
                {
                    // Scroll vers le haut : tile suivante
                    CurrentTileIndex++;
                    if (CurrentTileIndex >= AvailableTiles.Length)
                    {
                        CurrentTileIndex = 0; // Boucler au début
                    }
                }
                else
                {
                    // Scroll vers le bas : tile précédente
                    CurrentTileIndex--;
                    if (CurrentTileIndex < 0)
                    {
                        CurrentTileIndex = AvailableTiles.Length - 1; // Boucler à la fin
                    }
                }
                
                scrollAccumulator = 0f;
            }
        }
    }
    
    // Obtenir la tile actuellement sélectionnée
    public GameObject GetCurrentTilePrefab()
    {
        if (AvailableTiles == null || AvailableTiles.Length == 0)
        {
            return null;
        }
        
        CurrentTileIndex = Mathf.Clamp(CurrentTileIndex, 0, AvailableTiles.Length - 1);
        return AvailableTiles[CurrentTileIndex].tilePrefab;
    }
    
    // Obtenir le nom de la tile actuellement sélectionnée
    public string GetCurrentTileName()
    {
        if (AvailableTiles == null || AvailableTiles.Length == 0)
        {
            return "None";
        }
        
        CurrentTileIndex = Mathf.Clamp(CurrentTileIndex, 0, AvailableTiles.Length - 1);
        return AvailableTiles[CurrentTileIndex].tileName;
    }
    
    // Obtenir l'index de la tile actuelle
    public int GetCurrentTileIndex()
    {
        return CurrentTileIndex;
    }
    
    // Définir manuellement la tile sélectionnée
    public void SetCurrentTileIndex(int index)
    {
        if (AvailableTiles != null && AvailableTiles.Length > 0)
        {
            CurrentTileIndex = Mathf.Clamp(index, 0, AvailableTiles.Length - 1);
        }
    }
    
    // Obtenir le nombre total de tiles disponibles
    public int GetTileCount()
    {
        return AvailableTiles != null ? AvailableTiles.Length : 0;
    }
    
    // Afficher les informations de la tile sélectionnée
    private void OnGUI()
    {
        if (showTileInfo && AvailableTiles != null && AvailableTiles.Length > 0)
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
            GUI.Label(new Rect(x, y + lineHeight, width, lineHeight), $"Tile {CurrentTileIndex + 1}/{AvailableTiles.Length}", style);
            GUI.Label(new Rect(x, y + lineHeight * 2, width, lineHeight), "Scroll to change tile", style);
        }
    }
}

