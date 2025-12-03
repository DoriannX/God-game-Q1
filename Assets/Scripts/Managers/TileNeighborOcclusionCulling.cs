using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Système d'occlusion culling basé sur les voisins
/// Désactive les tiles complètement entourées par d'autres tiles (pas de raycast, ultra-performant)
/// </summary>
public class TileNeighborOcclusionCulling : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool enableOcclusion = true;
    [SerializeField] private bool occludeBaseTiles = true; // Occlure les tiles de base entourées
    [SerializeField] private bool occludeHeightTiles = true; // Occlure les tiles en hauteur entourées
    
    [Header("References")]
    [SerializeField] private TileHeightManager heightManager;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    
    // Cache des états d'occlusion par coordonnée et hauteur
    private Dictionary<Vector3Int, bool> occlusionCache = new Dictionary<Vector3Int, bool>();
    
    // Batch processing
    private bool isBatchMode = false;
    private HashSet<Vector2Int> batchedColumns = new HashSet<Vector2Int>();
    
    // Stats
    private int totalOccludedTiles = 0;
    private int totalVisibleTiles = 0;
    
    // Les 6 directions hexagonales (flat-top)
    private readonly Vector2Int[] hexDirections = new Vector2Int[]
    {
        new Vector2Int(1, 0),   // Est
        new Vector2Int(-1, 0),  // Ouest
        new Vector2Int(0, 1),   // Nord-Est
        new Vector2Int(0, -1),  // Sud-Ouest
        new Vector2Int(1, -1),  // Sud-Est
        new Vector2Int(-1, 1)   // Nord-Ouest
    };
    
    private void Start()
    {
        if (heightManager == null)
        {
            Debug.LogWarning("TileNeighborOcclusionCulling: HeightManager not assigned!");
        }
    }
    
    /// <summary>
    /// Vérifie et met à jour l'occlusion pour une colonne et ses voisins
    /// </summary>
    public void UpdateOcclusionForColumn(Vector2Int hexCoords)
    {
        if (!enableOcclusion || heightManager == null)
            return;
        
        // En mode batch, on accumule juste les colonnes à traiter
        if (isBatchMode)
        {
            batchedColumns.Add(hexCoords);
            foreach (Vector2Int direction in hexDirections)
            {
                batchedColumns.Add(hexCoords + direction);
            }
            return;
        }
        
        // Mettre à jour la colonne elle-même
        UpdateColumnOcclusion(hexCoords);
        
        // Mettre à jour les 6 voisins (car cette colonne peut maintenant les occlure)
        foreach (Vector2Int direction in hexDirections)
        {
            Vector2Int neighborCoords = hexCoords + direction;
            UpdateColumnOcclusion(neighborCoords);
        }
    }
    
    /// <summary>
    /// Démarre le mode batch pour optimiser les updates massifs
    /// </summary>
    public void BeginBatch()
    {
        isBatchMode = true;
        batchedColumns.Clear();
    }
    
    /// <summary>
    /// Termine le mode batch et traite toutes les colonnes accumulées
    /// </summary>
    public void EndBatch()
    {
        isBatchMode = false;
        
        // Traiter toutes les colonnes accumulées
        foreach (Vector2Int coords in batchedColumns)
        {
            UpdateColumnOcclusion(coords);
        }
        
        batchedColumns.Clear();
    }
    
    /// <summary>
    /// Met à jour l'occlusion d'une colonne complète
    /// </summary>
    private void UpdateColumnOcclusion(Vector2Int hexCoords)
    {
        int columnHeight = heightManager.GetColumnHeight(hexCoords);
        if (columnHeight == 0)
            return;
        
        // Vérifier chaque tile de la colonne
        for (int height = 0; height < columnHeight; height++)
        {
            bool shouldOcclude = ShouldOccludeTile(hexCoords, height, columnHeight);
            ApplyOcclusion(hexCoords, height, shouldOcclude);
        }
    }
    
    /// <summary>
    /// Détermine si une tile devrait être occludée
    /// </summary>
    private bool ShouldOccludeTile(Vector2Int hexCoords, int tileHeight, int columnHeight)
    {
        // Ne pas occlure la tile du dessus (toujours visible)
        if (tileHeight == columnHeight - 1)
            return false;
        
        // Vérifier le type de tile
        if (tileHeight == 0 && !occludeBaseTiles)
            return false;
        
        if (tileHeight > 0 && !occludeHeightTiles)
            return false;
        
        // Vérifier si tous les voisins horizontaux ont au moins la même hauteur
        bool allNeighborsHigherOrEqual = true;
        
        foreach (Vector2Int direction in hexDirections)
        {
            Vector2Int neighborCoords = hexCoords + direction;
            int neighborHeight = heightManager.GetColumnHeight(neighborCoords);
            
            // Si le voisin n'est pas assez haut, cette tile est visible
            if (neighborHeight <= tileHeight)
            {
                allNeighborsHigherOrEqual = false;
                break;
            }
        }
        
        // Vérifier qu'il y a une tile au-dessus (occlusionée par le dessus aussi)
        bool hasTileAbove = tileHeight < columnHeight - 1;
        
        // Occlure seulement si complètement entourée ET qu'il y a une tile au-dessus
        return allNeighborsHigherOrEqual && hasTileAbove;
    }
    
    /// <summary>
    /// Applique ou retire l'occlusion d'une tile
    /// </summary>
    private void ApplyOcclusion(Vector2Int hexCoords, int height, bool shouldOcclude)
    {
        Vector3Int key = new Vector3Int(hexCoords.x, height, hexCoords.y);
        
        // Vérifier si l'état a changé
        if (occlusionCache.TryGetValue(key, out bool cachedState))
        {
            if (cachedState == shouldOcclude)
                return; // Pas de changement
        }
        
        // Obtenir la tile
        List<GameObject> columnTiles = heightManager.GetColumnTiles(hexCoords);
        if (height >= columnTiles.Count || columnTiles[height] == null)
            return;
        
        GameObject tile = columnTiles[height];
        
        // Désactiver/activer les renderers
        Renderer[] renderers = tile.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.enabled = !shouldOcclude;
            }
        }
        
        // Mettre à jour le cache et les stats
        occlusionCache[key] = shouldOcclude;
        
        if (shouldOcclude)
        {
            totalOccludedTiles++;
            if (cachedState == false) totalVisibleTiles--;
        }
        else
        {
            if (cachedState == true) totalOccludedTiles--;
            totalVisibleTiles++;
        }
    }
    
    /// <summary>
    /// Retire une colonne du système d'occlusion
    /// </summary>
    public void RemoveColumn(Vector2Int hexCoords)
    {
        int columnHeight = heightManager.GetColumnHeight(hexCoords);
        
        // Nettoyer le cache
        for (int height = 0; height < columnHeight; height++)
        {
            Vector3Int key = new Vector3Int(hexCoords.x, height, hexCoords.y);
            if (occlusionCache.ContainsKey(key))
            {
                if (occlusionCache[key] == true)
                    totalOccludedTiles--;
                else
                    totalVisibleTiles--;
                    
                occlusionCache.Remove(key);
            }
        }
        
        // Mettre à jour les voisins (ils ne sont peut-être plus occludés)
        foreach (Vector2Int direction in hexDirections)
        {
            Vector2Int neighborCoords = hexCoords + direction;
            UpdateColumnOcclusion(neighborCoords);
        }
    }
    
    /// <summary>
    /// Recalcule l'occlusion de toutes les colonnes
    /// </summary>
    public void RecalculateAllOcclusion()
    {
        if (heightManager == null)
            return;
        
        // Réinitialiser le cache
        occlusionCache.Clear();
        totalOccludedTiles = 0;
        totalVisibleTiles = 0;
        
        // Obtenir toutes les colonnes
        List<Vector2Int> allCoords = heightManager.GetAllColumnCoordinates();
        
        foreach (Vector2Int coords in allCoords)
        {
            UpdateColumnOcclusion(coords);
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"Recalculated occlusion for {allCoords.Count} columns. Occluded: {totalOccludedTiles}, Visible: {totalVisibleTiles}");
        }
    }
    
    /// <summary>
    /// Active/désactive l'occlusion culling
    /// </summary>
    public void SetOcclusionEnabled(bool enabled)
    {
        if (enableOcclusion == enabled)
            return;
        
        enableOcclusion = enabled;
        
        if (enabled)
        {
            RecalculateAllOcclusion();
        }
        else
        {
            // Réactiver toutes les tiles
            foreach (var kvp in occlusionCache)
            {
                Vector3Int key = kvp.Key;
                Vector2Int hexCoords = new Vector2Int(key.x, key.z);
                int height = key.y;
                
                ApplyOcclusion(hexCoords, height, false);
            }
        }
    }
    
    // Afficher les stats en haut à droite
    private void OnGUI()
    {
        if (showDebugInfo && enableOcclusion)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 14;
            style.alignment = TextAnchor.UpperRight;
            
            float width = 300f;
            float lineHeight = 25f;
            float x = Screen.width - width - 10f;
            float y = 80f;
            
            int totalTiles = totalOccludedTiles + totalVisibleTiles;
            float occlusionPercent = totalTiles > 0 ? (totalOccludedTiles * 100f / totalTiles) : 0f;
            
            GUI.Label(new Rect(x, y, width, lineHeight), $"Neighbor Occlusion", style);
            GUI.Label(new Rect(x, y + lineHeight, width, lineHeight), $"Visible: {totalVisibleTiles} / {totalTiles}", style);
            GUI.Label(new Rect(x, y + lineHeight * 2, width, lineHeight), $"Occluded: {totalOccludedTiles} ({occlusionPercent:F1}%)", style);
        }
    }
}

