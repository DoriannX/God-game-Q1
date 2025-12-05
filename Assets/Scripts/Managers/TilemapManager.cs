using System;
using UnityEngine;
using System.Collections.Generic;

public class TilemapManager : MonoBehaviour
{
    [SerializeField] private TilePool tilePool; // Référence au pool de tiles
    [SerializeField] private float tileHeight = .2f; // Hauteur d'une tile individuelle
    [SerializeField] private float hexSize = 1f; // Largeur de l'hexagone (distance entre côtés opposés pour flat-top)
    [SerializeField] private BrushSizeManager brushManager; // Référence au BrushSizeManager
    [SerializeField] private TileSelector tileSelector; // Référence au TileSelector
    [SerializeField] private int maxHeight = 10; // Hauteur maximale d'une colonne (nombre de tiles)

    [SerializeField]
    private TileNeighborOcclusionCulling neighborOcclusion; // Référence au système d'occlusion (optionnel)

    [SerializeField] private float clickCooldown = 0.1f; // Temps minimum entre deux placements (en secondes)
    [SerializeField] private bool allowReplacement = true; // Permettre le remplacement des tiles existantes
    private Camera mainCamera;
    public Dictionary<Vector3Int, GameObject> tiles { get; private set; } = new();
    
    // Track the height of each column for O(1) lookup - key is (q, r), value is max height
    private Dictionary<Vector2Int, int> columnHeights = new();
    
    private float lastClickTime;
    private float lastRightClickTime;
    public Vector3Int hexCoords { get; private set;}

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

    /// <summary>
    /// Returns the bounds of all spawned tiles in the scene.
    /// Calculates min/max hex coordinates and converts to world space bounds.
    /// </summary>
    public BoundsInt cellBounds
    {
        get
        {
            if (tiles.Count == 0)
                return new BoundsInt(Vector3Int.zero, new Vector3Int(0, 0, 0));

            // Find min and max hex coordinates
            int minQ = int.MaxValue;
            int maxQ = int.MinValue;
            int minR = int.MaxValue;
            int maxR = int.MinValue;

            foreach (var coord in tiles.Keys)
            {
                if (coord.x < minQ) minQ = coord.x;
                if (coord.x > maxQ) maxQ = coord.x;
                if (coord.y < minR) minR = coord.y;
                if (coord.y > maxR) maxR = coord.y;
            }

            // Convert hex bounds to world space bounds
            Vector3 minPos = HexAxialToWorld(new Vector3Int(minQ, minR, 0));
            Vector3 maxPos = HexAxialToWorld(new Vector3Int(maxQ, maxR, 0));

            // Calculate bounds size (add padding for hex size)
            Vector3Int min = new Vector3Int(
                Mathf.FloorToInt(minPos.x - hexSize),
                0,
                Mathf.FloorToInt(minPos.z - hexSize)
            );

            Vector3Int max = new Vector3Int(
                Mathf.CeilToInt(maxPos.x + hexSize),
                0,
                Mathf.CeilToInt(maxPos.z + hexSize)
            );

            Vector3Int size = max - min;

            return new BoundsInt(min, size);
        }
    }

    public event Action<Vector3Int> cellChanged;

    public static TilemapManager instance { get; private set; }
    private Vector2 mousePos;

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("No main camera found in the scene!");
        }

        if (tilePool == null)
        {
            Debug.LogError("TilePool is not assigned to TilemapManagerCopy!");
        }
    }

    private void Update()
    {
        /*// Clic gauche : Remplacer/placer la tile de base
        if (Input.GetMouseButton(0))
        {
            // Vérifier si le cooldown est écoulé
            if (Time.time >= lastClickTime + clickCooldown)
            {
                DetectMouseClick(false); // false = clic gauche (remplacer)
                lastClickTime = Time.time;
            }
        }

        // Clic droit : Ajouter en hauteur (ne remplace pas, ajoute par dessus)
        // MAIS pas si Shift est enfoncé (réservé pour baisser dans TileHeightManager)
        if (Input.GetMouseButton(1) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            // Vérifier si le cooldown est écoulé
            if (Time.time >= lastRightClickTime + clickCooldown)
            {
                DetectMouseClick(true); // true = clic droit (ajouter en hauteur)
                lastRightClickTime = Time.time;
            }
        }*/
        // Créer un raycast depuis la caméra vers la position de la souris
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        Vector3 worldPosition;

        // Effectuer le raycast
        if (Physics.Raycast(ray, out hit))
        {
            // Position exacte du clic en coordonnées world si un objet est touché
            worldPosition = hit.point;

            // Visualiser le raycast en vert jusqu'au point d'impact
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green, 2f);

            // Dessiner une croix à la position du hit
            DrawCross(worldPosition, Color.green, 0.5f, 2f);
        }
        else
        {
            // Si aucun objet n'est touché, calculer l'intersection avec le plan Y = 0
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float distance;

            if (groundPlane.Raycast(ray, out distance))
            {
                worldPosition = ray.GetPoint(distance);

                // Visualiser le raycast en cyan jusqu'au plan
                Debug.DrawRay(ray.origin, ray.direction * distance, Color.cyan, 2f);

                // Dessiner une croix à la position calculée
                DrawCross(worldPosition, Color.cyan, 0.5f, 2f);
            }
            else
            {
                // Visualiser le raycast en rouge si aucune intersection
                Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);
                Debug.LogWarning("Could not calculate world position");
                return;
            }
        }

        // Convertir la position world en coordonnées axiales hexagonales
        hexCoords = WorldToHexAxial(worldPosition);
        
    }
    
    public void SetMousePos(Vector2 pos)
    {
        mousePos = pos;
    }

    public void PlaceTile(GameObject tilePrefab)
    {
        DetectMouseClick(tilePrefab);
    }

    private void DetectMouseClick(GameObject tilePrefab = null)
    {

        var brushArea = brushManager.GetBrushArea(hexCoords);
        // Si on a beaucoup de tiles, utiliser le mode batch pour l'occlusion
        bool useBatch = brushArea.Length > 1 && neighborOcclusion != null;
        if (useBatch)
        {
            neighborOcclusion.BeginBatch();
        }
        
        foreach (var coord in brushArea)
        {
            // Get column height and spawn on top
            int height = GetColumnHeight(coord);
            SpawnTileAt(new Vector3Int(coord.x, coord.y, height+1), tilePrefab);
        }

        // Terminer le mode batch
        if (useBatch)
        {
            neighborOcclusion.EndBatch();
        }
    }

    public void SpawnTileAt(Vector3Int hexCoords, GameObject tilePrefab)
    {
        // Vérifier si une tile existe déjà à cette position
        if (tiles.ContainsKey(hexCoords))
        {
            if (!allowReplacement)
            {
                // Ne pas remplacer si le remplacement est désactivé
                return;
            }

            // Récupérer l'ancienne tile
            GameObject oldTile = tiles[hexCoords];

            // Détruire l'ancienne tile
            if (oldTile != null)
            {
                Destroy(oldTile);
            }

            // Retirer l'ancienne tile du dictionnaire
            tiles.Remove(hexCoords);
        }

        Vector3 spawnPosition = HexAxialToWorld(hexCoords);

        // Instancier la nouvelle tile (pas de pool car tiles différentes)
        GameObject tile = Instantiate(tilePrefab, spawnPosition, Quaternion.identity);
        tile.name = $"Tile_{tilePrefab.name}_({hexCoords.x}, {hexCoords.y}, {hexCoords.z})";

        // Ajouter la tile au dictionnaire
        tiles.Add(hexCoords, tile);

        // Update column height tracker
        Vector2Int columnKey = new Vector2Int(hexCoords.x, hexCoords.y);
        int newHeight = hexCoords.z + 1; // Next available position (z is 0-indexed, so z=0 means height=1)
        if (!columnHeights.ContainsKey(columnKey) || columnHeights[columnKey] < newHeight)
        {
            columnHeights[columnKey] = newHeight;
        }

        // Update occlusion for this column if the system is available
        if (neighborOcclusion != null)
        {
            neighborOcclusion.UpdateOcclusionForColumn(hexCoords);
        }

        // Notify listeners that cells have changed
        cellChanged?.Invoke(new Vector3Int(hexCoords.x, 0, hexCoords.y));
    }

    // Convertir une position world en coordonnées axiales hexagonales (flat-top)
    public Vector3Int WorldToHexAxial(Vector3 worldPosition)
    {
        // Pour flat-top hexagons avec largeur (width = distance entre côtés opposés):
        // La largeur correspond à 2 * taille_interne
        // Donc taille_interne = largeur / 2

        float x = worldPosition.x;
        float z = worldPosition.z;

        float sizeInternal = hexSize / 2f;

        float q = (2f / 3f * x) / sizeInternal;
        float r = (-1f / 3f * x + Mathf.Sqrt(3f) / 3f * z) / sizeInternal;

        // Arrondir aux coordonnées hexagonales entières
        return HexRound(q, r);
    }

    // Arrondir les coordonnées fractionnelles vers les coordonnées hexagonales entières
    private Vector3Int HexRound(float q, float r)
    {
        float s = -q - r; // coordonnée cubique s

        int roundedQ = Mathf.RoundToInt(q);
        int roundedR = Mathf.RoundToInt(r);
        int roundedS = Mathf.RoundToInt(s);

        float qDiff = Mathf.Abs(roundedQ - q);
        float rDiff = Mathf.Abs(roundedR - r);
        float sDiff = Mathf.Abs(roundedS - s);

        // Réajuster la coordonnée avec la plus grande différence
        if (qDiff > rDiff && qDiff > sDiff)
        {
            roundedQ = -roundedR - roundedS;
        }
        else if (rDiff > sDiff)
        {
            roundedR = -roundedQ - roundedS;
        }

        return new Vector3Int(roundedQ, roundedR, 0);
    }
    
    public Vector3 HexAxialToWorld(Vector3Int hexCoords)
    {
        // Pour flat-top hexagons avec largeur (width):
        // La largeur correspond à 2 * taille_interne
        // Donc taille_interne = largeur / 2

        float sizeInternal = hexSize / 2f;

        float x = sizeInternal * (3f / 2f * hexCoords.x);
        float z = sizeInternal * (Mathf.Sqrt(3f) / 2f * hexCoords.x + Mathf.Sqrt(3f) * hexCoords.y);
        
        return new Vector3(x, hexCoords.z * tileHeight, z);
    }

    // Dessiner une croix pour visualiser la position
    private void DrawCross(Vector3 position, Color color, float size, float duration)
    {
        // Croix sur le plan XY
        Debug.DrawLine(position + Vector3.left * size, position + Vector3.right * size, color, duration);
        Debug.DrawLine(position + Vector3.up * size, position + Vector3.down * size, color, duration);

        // Ligne verticale sur Z
        Debug.DrawLine(position + Vector3.forward * size, position + Vector3.back * size, color, duration);
    }

    public GameObject GetTile(Vector3Int cellPos)
    {
        tiles.TryGetValue(new Vector3Int(cellPos.x, cellPos.z, 0), out GameObject tile);
        return tile;
    }

    /// <summary>
    /// Gets the current height of a column at the given hex coordinates (q, r)
    /// Returns the next available height position (0 if no tiles exist)
    /// </summary>
    public int GetColumnHeight(Vector2Int hexCoords)
    {
        return columnHeights.GetValueOrDefault(hexCoords, 0);
    }

    public void RemoveTile()
    {
        var brushArea = brushManager.GetBrushArea(hexCoords);
        
        // Use batch mode for occlusion if removing multiple tiles
        bool useBatch = brushArea.Length > 1 && neighborOcclusion != null;
        if (useBatch)
        {
            neighborOcclusion.BeginBatch();
        }
        
        foreach (var coord in brushArea)
        {
            int height = GetColumnHeight(coord);
            if (height > 0)
            {
                // height is the next available position, so the top tile is at height - 1
                RemoveTileAt(new Vector3Int(coord.x, coord.y, height - 1));
            }
        }
        
        // End batch mode
        if (useBatch)
        {
            neighborOcclusion.EndBatch();
        }
    }
    
    public void RemoveTileAt(Vector3Int hexCoords)
    {
        // Vérifier si une tile existe à cette position
        if (!tiles.TryGetValue(hexCoords, out var tile))
            return;

        // Récupérer la tile

        // Détruire la tile
        if (tile != null)
        {
            Destroy(tile);
        }

        // Retirer la tile du dictionnaire
        tiles.Remove(hexCoords);

        // Mettre à jour la hauteur de la colonne de manière optimisée
        Vector2Int columnKey = new Vector2Int(hexCoords.x, hexCoords.y);
        
        if (columnHeights.TryGetValue(columnKey, out int currentMaxHeight))
        {
            // Si on a supprimé la tile au sommet de la colonne
            if (hexCoords.z == currentMaxHeight - 1)
            {
                // Chercher la nouvelle hauteur maximale en descendant
                // On ne vérifie que les hauteurs en dessous du sommet actuel
                int newMaxHeight = 0;
                for (int z = hexCoords.z - 1; z >= 0; z--)
                {
                    if (tiles.ContainsKey(new Vector3Int(hexCoords.x, hexCoords.y, z)))
                    {
                        newMaxHeight = z + 1;
                        break;
                    }
                }

                if (newMaxHeight == 0)
                {
                    columnHeights.Remove(columnKey);
                }
                else
                {
                    columnHeights[columnKey] = newMaxHeight;
                }
            }
            // Si on a supprimé une tile au milieu, la hauteur max ne change pas
            // (il y a toujours des tiles au-dessus)
        }
        
        
        if (neighborOcclusion != null)
        {
            neighborOcclusion.UpdateOcclusionForColumn(hexCoords);
        }

        // Notifier les auditeurs que les cellules ont changé
        cellChanged?.Invoke(new Vector3Int(hexCoords.x, 0, hexCoords.y));
    }
    
    public void RemoveTileAt(Vector3 position)
    {
        Vector3Int hexCoords = WorldToHexAxial(position);
        int height = GetColumnHeight(new Vector2Int(hexCoords.x, hexCoords.y));
        if (height > 0)
        {
            RemoveTileAt(new Vector3Int(hexCoords.x, hexCoords.y, height - 1));
        }
    }
}