using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    [SerializeField] private TilePool tilePool; // Référence au pool de tiles
    [SerializeField] private float hexSize = 1f; // Largeur de l'hexagone (distance entre côtés opposés pour flat-top)
    [SerializeField] private TileHeightManager heightManager; // Référence au TileHeightManager
    [SerializeField] private BrushSizeManager brushManager; // Référence au BrushSizeManager
    [SerializeField] private TileSelector tileSelector; // Référence au TileSelector

    [SerializeField]
    private TileNeighborOcclusionCulling neighborOcclusion; // Référence au système d'occlusion (optionnel)

    [SerializeField] private float clickCooldown = 0.1f; // Temps minimum entre deux placements (en secondes)
    [SerializeField] private bool allowReplacement = true; // Permettre le remplacement des tiles existantes

    private Camera mainCamera;
    private Dictionary<Vector3Int, GameObject> tiles = new();
    private float lastClickTime;
    private float lastRightClickTime;

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
            Vector3 minPos = HexAxialToWorld(minQ, minR);
            Vector3 maxPos = HexAxialToWorld(maxQ, maxR);

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

    public Vector2 cellSize =>
        new Vector2(hexSize * 0.75f, hexSize * Mathf.Sqrt(3) / 2f); // Taille d'une cellule hexagonale (flat-top)

    public static TilemapManager instance { get; private set; }

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
    }

    public void PlaceTile(Vector3 mousePos, GameObject tilePrefab)
    {
        DetectMouseClick(mousePos, false, tilePrefab);
    }
    
    public void RaiseTile(Vector3 mousePos)
    {
        DetectMouseClick(mousePos, true);
    }

    private void DetectMouseClick(Vector2 mousePos, bool isRightClick, GameObject tilePrefab = null)
    {
        
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
        Vector3Int hexCoords = WorldToHexAxial(worldPosition);

        // Obtenir la zone du brush
        Vector3Int[] brushArea;
        if (brushManager != null)
        {
            brushArea = brushManager.GetBrushArea(hexCoords);
        }
        else
        {
            // Si pas de brush manager, spawner seulement une tile
            brushArea = new Vector3Int[] { hexCoords };
        }

        // Si on a beaucoup de tiles, utiliser le mode batch pour l'occlusion
        bool useBatch = brushArea.Length > 1 && neighborOcclusion != null;
        if (useBatch)
        {
            neighborOcclusion.BeginBatch();
        }

        // Spawner les tiles dans la zone du brush
        foreach (Vector3Int coord in brushArea)
        {
            if (isRightClick)
            {
                // Clic droit : Ajouter en hauteur (utiliser le heightManager)
                if (heightManager != null)
                {
                    heightManager.RaiseColumnWithTileType(coord, GetCurrentTilePrefab());
                }
            }
            else
            {
                // Clic gauche : Remplacer/placer la tile de base
                SpawnTileAt(coord, tilePrefab);
            }
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

            // Retourner SEULEMENT les tiles de hauteur au pool, garder la base pour la remplacer
            if (heightManager != null)
            {
                heightManager.ResetColumnKeepBase(hexCoords, tilePool);
            }

            // Détruire l'ancienne tile de base
            if (oldTile != null)
            {
                Destroy(oldTile);
            }

            // Retirer l'ancienne tile du dictionnaire
            tiles.Remove(hexCoords);
        }

        Vector3 spawnPosition = HexAxialToWorld(hexCoords.x, hexCoords.y);

        // Instancier la nouvelle tile (pas de pool car tiles différentes)
        GameObject tile = Instantiate(tilePrefab, spawnPosition, Quaternion.identity);
        tile.name = $"Tile_{tilePrefab.name}_({hexCoords.x}, {hexCoords.y})";

        // Ajouter la tile au dictionnaire
        tiles.Add(hexCoords, tile);

        // Notifier le height manager de la nouvelle tile de base
        if (heightManager != null)
        {
            heightManager.RegisterBaseTile(hexCoords, tile);
        }

        // Notify listeners that cells have changed
        cellChanged?.Invoke(new Vector3Int(hexCoords.x, 0, hexCoords.y));
    }

    // Obtenir le prefab de la tile actuelle depuis le TileSelector
    private GameObject GetCurrentTilePrefab()
    {
        if (tileSelector != null)
        {
            return tileSelector.GetCurrentTilePrefab();
        }

        return null;
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

    // Convertir des coordonnées axiales hexagonales en position world (si nécessaire)
    private Vector3 HexAxialToWorld(int q, int r)
    {
        // Pour flat-top hexagons avec largeur (width):
        // La largeur correspond à 2 * taille_interne
        // Donc taille_interne = largeur / 2

        float sizeInternal = hexSize / 2f;

        float x = sizeInternal * (3f / 2f * q);
        float z = sizeInternal * (Mathf.Sqrt(3f) / 2f * q + Mathf.Sqrt(3f) * r);

        return new Vector3(x, 0f, z);
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

    public Vector3 GetCellCenterWorld(Vector3Int cellPos)
    {
        return HexAxialToWorld(cellPos.x, cellPos.z);
    }
}