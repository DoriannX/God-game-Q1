/*
using UnityEngine;
using System.Collections.Generic;

public class TileHeightManager : MonoBehaviour
{
    [SerializeField] private TilePool tilePool; // Référence au pool de tiles
    [SerializeField] private float clickCooldown = 0.1f; // Temps minimum entre deux clics (en secondes)
    [SerializeField] private BrushSizeManager brushManager; // Référence au BrushSizeManager
    [SerializeField] private TileSelector tileSelector; // Référence au TileSelector
    [SerializeField] private TileNeighborOcclusionCulling neighborOcclusion; // Référence au système d'occlusion par voisinage (optionnel)
    [SerializeField] private float hexSize = 1f; // Largeur de l'hexagone (doit correspondre à TilemapManagerCopy)
    
    private Camera mainCamera;
    // Dictionnaire qui stocke les colonnes de tilesPosition par coordonnées hexagonales
    // La clé est (q, r), la valeur est une liste de GameObjects empilés
    private float lastClickTime; // Temps du dernier clic
    private Dictionary<Vector3Int, float> lastPlacementTime = new Dictionary<Vector3Int, float>(); // Temps du dernier placement par coordonnée
    
    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("No main camera found in the scene!");
        }
        
        if (tilePool == null)
        {
            Debug.LogError("TilePool is not assigned to TileHeightManager!");
        }
    }
    
    private void Update()
    {
        // Note: La gestion des clics est maintenant dans TilemapManagerCopy
        // Clic gauche = remplacer/placer tile de base
        // Clic droit = ajouter en hauteur (appelle RaiseColumnWithTileType)
        // Shift + clic droit = baisser (géré par TilemapManagerCopy si nécessaire)
        
        // On garde la possibilité de shift+clic droit pour baisser directement depuis ce script
        /*if (Input.GetMouseButton(1) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            if (Time.time >= lastClickTime + clickCooldown)
            {
                DetectTileClickToLower();
                lastClickTime = Time.time;
            }
        }#1#
    }
    
    private void DetectTileClick()
    {
        /#1#/ Obtenir la position de la souris à l'écran
        Vector3 mouseScreenPosition = Input.mousePosition;
        
        // Créer un raycast depuis la caméra vers la position de la souris
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);
        
        // Calculer l'intersection avec le plan Y = 0 (comme BrushPreview)
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float distance;
        
        if (groundPlane.Raycast(ray, out distance))
        {
            Vector3 worldPosition = ray.GetPoint(distance);
            
            // Visualiser le raycast en jaune
            Debug.DrawRay(ray.origin, ray.direction * distance, Color.yellow, 2f);
            
            // Debug.Log désactivé pour éviter le spam
            // Debug.Log($"Click at world position: {worldPosition}, hex coords: ({hexCoords.x}, {hexCoords.y})");
            
            // Convertir la position world en coordonnées hexagonales (comme BrushPreview)
            Vector3Int hexCoords = WorldToHexAxial(worldPosition);
            
            // Debug.Log désactivé pour éviter le spam
            // Debug.Log($"Raising {brushArea.Length} columns with brush size {brushManager.GetBrushSize()}");
            
            // Obtenir la zone du brush
            Vector3Int[] brushArea;
            if (brushManager != null)
            {
                brushArea = brushManager.GetBrushArea(hexCoords);
                // Debug.Log désactivé pour éviter le spam
                // Debug.Log($"Raising {brushArea.Length} columns with brush size {brushManager.GetBrushSize()}");
            }
            else
            {
                // Si pas de brush manager, monter seulement une colonne
                brushArea = new Vector3Int[] { hexCoords };
            }
            
            // Monter toutes les colonnes dans la zone du brush
            foreach (Vector3Int coord in brushArea)
            {
                RaiseColumn(coord);
            }
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);
        }#1#
    }
    
    private void DetectTileClickToLower()
    {
        // Obtenir la position de la souris à l'écran
        Vector3 mouseScreenPosition = Input.mousePosition;
        
        // Créer un raycast depuis la caméra vers la position de la souris
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);
        
        // Calculer l'intersection avec le plan Y = 0 (comme BrushPreview)
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float distance;
        
        if (groundPlane.Raycast(ray, out distance))
        {
            Vector3 worldPosition = ray.GetPoint(distance);
            
            // Visualiser le raycast en magenta
            Debug.DrawRay(ray.origin, ray.direction * distance, Color.magenta, 2f);
            
            // Convertir la position world en coordonnées hexagonales (comme BrushPreview)
            Vector3Int hexCoords = WorldToHexAxial(worldPosition);
            
            // Debug.Log désactivé pour éviter le spam
            // Debug.Log($"Shift+Click at world position: {worldPosition}, hex coords: ({hexCoords.x}, {hexCoords.y})");
            
            // Obtenir la zone du brush
            Vector3Int[] brushArea;
            if (brushManager != null)
            {
                brushArea = brushManager.GetBrushArea(hexCoords);
            }
            else
            {
                // Si pas de brush manager, baisser seulement une colonne
                brushArea = new Vector3Int[] { hexCoords };
            }
            
            // Si on a beaucoup de tilesPosition, utiliser le mode batch pour l'occlusion
            bool useBatch = brushArea.Length > 1 && neighborOcclusion != null;
            if (useBatch)
            {
                neighborOcclusion.BeginBatch();
            }
            
            // Baisser toutes les colonnes dans la zone du brush
            foreach (Vector3Int coord in brushArea)
            {
                LowerColumn(coord);
            }
            
            // Terminer le mode batch
            if (useBatch)
            {
                neighborOcclusion.EndBatch();
            }
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);
        }
    }
    
    private void LowerColumn(Vector3Int hexCoords)
    {
        // Vérifier le cooldown pour cette coordonnée spécifique
        if (lastPlacementTime.ContainsKey(hexCoords))
        {
            if (Time.time < lastPlacementTime[hexCoords] + clickCooldown)
            {
                return; // Trop tôt, on skip cette coordonnée
            }
        }
        
        // Vérifier si la colonne existe
        if (!tileColumns.ContainsKey(hexCoords))
        {
            return;
        }
        
        List<GameObject> column = tileColumns[hexCoords];
        
        // Vérifier qu'il y a au moins 2 tilesPosition (base + une au-dessus)
        // On ne peut pas détruire la tile de base (index 0)
        if (column.Count <= 1)
        {
            return;
        }
        
        // Obtenir la dernière tile (au sommet de la colonne)
        int lastIndex = column.Count - 1;
        GameObject tileToRemove = column[lastIndex];
        
        // Détruire la tile (pas de pool car différentes tilesPosition)
        if (tileToRemove != null)
        {
            Destroy(tileToRemove);
        }
        
        // Retirer la tile de la liste
        column.RemoveAt(lastIndex);
        
        // Enregistrer le temps du placement pour cette coordonnée
        lastPlacementTime[hexCoords] = Time.time;
        
        // Notifier le système d'occlusion par voisinage
        if (neighborOcclusion != null)
        {
            neighborOcclusion.UpdateOcclusionForColumn(hexCoords);
        }
    }
    
    // Méthode publique pour obtenir la hauteur d'une colonne
    public int GetColumnHeight(Vector3Int hexCoords)
    {
        if (tileColumns.ContainsKey(hexCoords))
        {
            return tileColumns[hexCoords].Count;
        }
        return 0;
    }
    
    // Méthode publique pour obtenir la position Y où la prochaine tile sera placée
    public float GetNextTileYPosition(Vector3Int hexCoords)
    {
        if (!tileColumns.ContainsKey(hexCoords))
        {
            return 0f; // Pas de colonne = position au sol
        }
        
        List<GameObject> column = tileColumns[hexCoords];
        if (column.Count == 0 || column[0] == null)
        {
            return 0f; // Pas de tile de base = position au sol
        }
        
        // Obtenir la position de la tile de base
        GameObject baseTile = column[0];
        Vector3 basePosition = baseTile.transform.position;
        
        // Calculer la position Y de la prochaine tile (comme dans RaiseColumnWithTileType)
        float nextY = basePosition.y + (column.Count) * tileHeight;
        return nextY;
    }
    
    // Méthode publique pour obtenir la position Y de la dernière tile dans la colonne (pour la preview)
    public float GetTopTileYPosition(Vector3Int hexCoords)
    {
        if (!tileColumns.ContainsKey(hexCoords))
        {
            return 0f; // Pas de colonne = position au sol
        }
        
        List<GameObject> column = tileColumns[hexCoords];
        if (column.Count == 0 || column[0] == null)
        {
            return 0f; // Pas de tile de base = position au sol
        }
        
        // Obtenir la position de la dernière tile dans la colonne
        GameObject lastTile = column[column.Count - 1];
        if (lastTile != null)
        {
            return lastTile.transform.position.y;
        }
        
        // Fallback : calculer à partir de la base
        GameObject baseTile = column[0];
        Vector3 basePosition = baseTile.transform.position;
        return basePosition.y + ((column.Count - 1) * tileHeight);
    }
    
    // Méthode publique pour enregistrer une tile de base créée par le TilemapManager
    public void RegisterBaseTile(Vector3Int hexCoords, GameObject baseTile)
    {
        // Initialiser la colonne si elle n'existe pas encore
        if (!tileColumns.ContainsKey(hexCoords))
        {
            tileColumns[hexCoords] = new List<GameObject>();
            tileColumns[hexCoords].Add(baseTile);
        }
        else
        {
            // La colonne existe déjà, remplacer la tile de base (index 0)
            if (tileColumns[hexCoords].Count > 0)
            {
                tileColumns[hexCoords][0] = baseTile; // Remplacer l'index 0
            }
            else
            {
                tileColumns[hexCoords].Add(baseTile); // Si la liste est vide (ne devrait pas arriver)
            }
        }
        
        // Notifier le système d'occlusion par voisinage
        if (neighborOcclusion != null)
        {
            neighborOcclusion.UpdateOcclusionForColumn(hexCoords);
        }
    }
    
    // Méthode publique pour réinitialiser une colonne (détruire toutes les tilesPosition au-dessus de la base)
    public void ResetColumn(Vector3Int hexCoords)
    {
        if (tileColumns.ContainsKey(hexCoords))
        {
            List<GameObject> column = tileColumns[hexCoords];
            
            // Détruire toutes les tilesPosition de la colonne sauf la première (tile de base)
            for (int i = 1; i < column.Count; i++)
            {
                if (column[i] != null && tilePool != null)
                {
                    tilePool.ReleaseTile(column[i]);
                }
            }
            
            // Vider la liste
            column.Clear();
            tileColumns.Remove(hexCoords);
        }
    }
    
    // Méthode publique pour reset ET clear toute la colonne (y compris la base)
    public void ResetAndClearColumn(Vector3Int hexCoords, TilePool pool)
    {
        if (tileColumns.ContainsKey(hexCoords))
        {
            List<GameObject> column = tileColumns[hexCoords];
            
            // Retourner TOUTES les tilesPosition au pool (y compris la base, index 0)
            // SAUF la base qui sera gérée par TilemapManagerCopy
            for (int i = 1; i < column.Count; i++)
            {
                if (column[i] != null && pool != null)
                {
                    pool.ReleaseTile(column[i]);
                }
            }
            
            // Vider complètement la colonne
            column.Clear();
            tileColumns.Remove(hexCoords);
        }
    }
    
    // Méthode optimisée : reset seulement les tilesPosition au-dessus, GARDER la base
    public void ResetColumnKeepBase(Vector3Int hexCoords, TilePool pool)
    {
        if (!tileColumns.ContainsKey(hexCoords))
        {
            return; // Pas de colonne = rien à faire
        }
        
        List<GameObject> column = tileColumns[hexCoords];
        
        // OPTIMISATION : Si la colonne n'a que la base (count == 1), ne rien faire !
        if (column.Count <= 1)
        {
            return; // Déjà resetée, évite le lag
        }
        
        // Détruire SEULEMENT les tilesPosition au-dessus de la base (index 1+)
        // On détruit au lieu d'utiliser le pool car ce sont des instances de prefabs différents
        for (int i = column.Count - 1; i >= 1; i--)
        {
            if (column[i] != null)
            {
                Destroy(column[i]);
            }
        }
        
        // Garder la tile de base (index 0), retirer le reste
        GameObject baseTile = column[0];
        column.Clear();
        column.Add(baseTile); // Remettre seulement la base
    }
    
    // Méthode publique pour ajouter une tile en hauteur avec un type spécifique
    public void RaiseColumnWithTileType(Vector3Int hexCoords, GameObject tilePrefab)
    {
        // Vérifier le cooldown pour cette coordonnée spécifique
        if (lastPlacementTime.ContainsKey(hexCoords))
        {
            if (Time.time < lastPlacementTime[hexCoords] + clickCooldown)
            {
                return; // Trop tôt, on skip cette coordonnée
            }
        }
        
        if (tilePrefab == null)
        {
            return;
        }
        
        // Vérifier que la colonne existe (qu'il y a une tile de base)
        if (!tileColumns.ContainsKey(hexCoords))
        {
            return;
        }
        
        List<GameObject> column = tileColumns[hexCoords];
        
        // Vérifier si la hauteur maximale est atteinte
        if (column.Count >= maxHeight)
        {
            return; // Ne pas monter plus haut que maxHeight
        }
        
        // Obtenir la position de la tile de base pour avoir les bonnes coordonnées X et Z
        GameObject baseTile = column[0];
        if (baseTile == null)
        {
            return;
        }
        
        Vector3 basePosition = baseTile.transform.position;
        
        // Calculer la hauteur Y pour la nouvelle tile
        // On part de la position Y de la base et on ajoute la hauteur en fonction du nombre de tilesPosition
        float newTileY = basePosition.y + (column.Count) * tileHeight;
        
        // Utiliser la position X et Z de la base, mais avec la nouvelle hauteur Y
        Vector3 spawnPosition = new Vector3(basePosition.x, newTileY, basePosition.z);
        
        // Instancier le prefab spécifique (pas de pool car différentes tilesPosition)
        GameObject newTile = Instantiate(tilePrefab, spawnPosition, Quaternion.identity);
        newTile.name = $"Tile_{tilePrefab.name}_({hexCoords.x}, {hexCoords.y})_H{column.Count}";
        

        // Ajouter la tile à la colonne
        column.Add(newTile);
        
        // Enregistrer le temps du placement pour cette coordonnée
        lastPlacementTime[hexCoords] = Time.time;
        
        // Notifier le système d'occlusion par voisinage
        if (neighborOcclusion != null)
        {
            neighborOcclusion.UpdateOcclusionForColumn(hexCoords);
        }
    }
    
    // Convertir une position world en coordonnées axiales hexagonales (flat-top)
    private Vector3Int WorldToHexAxial(Vector3 worldPosition)
    {
        float x = worldPosition.x;
        float z = worldPosition.z;
        
        float sizeInternal = hexSize / 2f;
        
        float q = (2f / 3f * x) / sizeInternal;
        float r = (-1f / 3f * x + Mathf.Sqrt(3f) / 3f * z) / sizeInternal;
        
        return HexRound(q, r);
    }
    
    // Arrondir les coordonnées fractionnelles vers les coordonnées hexagonales entières
    private Vector3Int HexRound(float q, float r)
    {
        float s = -q - r;
        
        int roundedQ = Mathf.RoundToInt(q);
        int roundedR = Mathf.RoundToInt(r);
        int roundedS = Mathf.RoundToInt(s);
        
        float qDiff = Mathf.Abs(roundedQ - q);
        float rDiff = Mathf.Abs(roundedR - r);
        float sDiff = Mathf.Abs(roundedS - s);
        
        if (qDiff > rDiff && qDiff > sDiff)
        {
            roundedQ = -roundedR - roundedS;
        }
        else if (rDiff > sDiff)
        {
            roundedR = -roundedQ - roundedS;
        }
        
        return new Vector3Int(roundedQ, roundedR);
    }
    
    // ===== Méthodes publiques pour l'occlusion par voisinage =====
    
    /// <summary>
    /// Obtenir toutes les coordonnées des colonnes existantes
    /// </summary>
    public List<Vector3Int> GetAllColumnCoordinates()
    {
        return new List<Vector3Int>(tileColumns.Keys);
    }
    
    /// <summary>
    /// Obtenir les tilesPosition d'une colonne
    /// </summary>
    public List<GameObject> GetColumnTiles(Vector3Int hexCoords)
    {
        if (tileColumns.ContainsKey(hexCoords))
        {
            return new List<GameObject>(tileColumns[hexCoords]);
        }
        return new List<GameObject>();
    }
}
*/
