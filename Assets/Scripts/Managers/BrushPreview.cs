using UnityEngine;
using System.Collections.Generic;

public class BrushPreview : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BrushSizeManager brushManager;
    [SerializeField] private TileHeightManager heightManager; // Pour obtenir la hauteur des colonnes
    [SerializeField] private TileSelector tileSelector; // Pour obtenir la tile sélectionnée
    [SerializeField] private Material previewMaterialTemplate; // Matériau transparent à utiliser pour la preview
    
    [Header("Preview Settings")]
    [SerializeField] private Color previewColor = new Color(0f, 1f, 0f, 0.5f); // Vert semi-transparent
    [SerializeField] private float hexSize = 1f; // Doit correspondre à la taille dans TilemapManagerCopy
    [SerializeField] private float tileHeight = 0.2f; // Hauteur d'une tile (doit correspondre à TileHeightManager)
    [SerializeField] private float previewHeight = 0.01f; // Hauteur au-dessus de la colonne pour éviter le z-fighting
    [SerializeField] private bool showPreview = true;
    
    private Camera mainCamera;
    private Material previewMaterial;
    private List<GameObject> previewObjects = new List<GameObject>();
    private Vector2Int lastCenterHex = new Vector2Int(int.MinValue, int.MinValue);
    private int lastBrushSize = -1;
    private int lastTileIndex = -1; // Pour détecter le changement de tile
    private float updateInterval = 0.1f; // Intervalle de mise à jour automatique
    private float lastUpdateTime = 0f;
    
    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("No main camera found in the scene!");
        }
        
        if (brushManager == null)
        {
            Debug.LogWarning("BrushManager is not assigned to BrushPreview!");
        }
        
        if (heightManager == null)
        {
            Debug.LogWarning("HeightManager is not assigned to BrushPreview! Preview will stay at ground level.");
        }
        
        if (tileSelector == null)
        {
            Debug.LogWarning("TileSelector is not assigned to BrushPreview!");
        }
        
        if (previewMaterialTemplate == null)
        {
            Debug.LogWarning("PreviewMaterialTemplate is not assigned! Preview may not work correctly.");
        }
    }
    
    private void Update()
    {
        if (!showPreview || brushManager == null)
            return;
        
        // Obtenir la position de la souris
        Vector3 mouseScreenPosition = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);
        
        // Calculer l'intersection avec le plan Y = 0
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float distance;
        
        if (groundPlane.Raycast(ray, out distance))
        {
            Vector3 worldPosition = ray.GetPoint(distance);
            
            // Convertir en coordonnées hexagonales
            Vector2Int centerHex = WorldToHexAxial(worldPosition);
            int currentBrushSize = brushManager.GetBrushSize();
            int currentTileIndex = tileSelector != null ? tileSelector.GetCurrentTileIndex() : -1;
            
            // Vérifier si on doit mettre à jour (position/taille/tile changée OU intervalle écoulé)
            bool positionChanged = centerHex != lastCenterHex || currentBrushSize != lastBrushSize;
            bool tileChanged = currentTileIndex != lastTileIndex;
            bool intervalElapsed = Time.time >= lastUpdateTime + updateInterval;
            
            if (positionChanged || tileChanged || intervalElapsed)
            {
                UpdatePreview(centerHex);
                lastCenterHex = centerHex;
                lastBrushSize = currentBrushSize;
                lastTileIndex = currentTileIndex;
                lastUpdateTime = Time.time;
            }
        }
        else
        {
            // Cacher la préview si pas d'intersection
            ClearPreview();
        }
    }
    
    private void UpdatePreview(Vector2Int centerHex)
    {
        // Obtenir la zone du brush
        Vector2Int[] brushArea = brushManager.GetBrushArea(centerHex);
        
        // Obtenir l'index de la tile actuelle
        int currentTileIndex = tileSelector != null ? tileSelector.GetCurrentTileIndex() : -1;
        
        // Si la tile a changé, détruire tous les objets existants pour les recréer avec le nouveau prefab
        if (currentTileIndex != lastTileIndex && lastTileIndex != -1)
        {
            foreach (GameObject obj in previewObjects)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            previewObjects.Clear();
        }
        
        // Réutiliser ou créer les objets de préview
        for (int i = 0; i < brushArea.Length; i++)
        {
            Vector3 hexWorldPos = HexAxialToWorld(brushArea[i].x, brushArea[i].y);
            
            // Obtenir la hauteur de la colonne à cette position
            if (heightManager != null)
            {
                int columnHeight = heightManager.GetColumnHeight(brushArea[i]);
                
                if (columnHeight == 0)
                {
                    // Pas de colonne : preview au sol exactement à Y=0 (sans offset)
                    hexWorldPos.y = 0f;
                }
                else
                {
                    // Il y a une colonne : afficher la preview au niveau de la dernière tile
                    float topTileY = heightManager.GetTopTileYPosition(brushArea[i]);
                    hexWorldPos.y = topTileY + previewHeight;
                }
            }
            else
            {
                // Pas de height manager : preview au sol
                hexWorldPos.y = 0f;
            }
            
            if (i < previewObjects.Count && previewObjects[i] != null)
            {
                // Réutiliser un objet existant
                previewObjects[i].transform.position = hexWorldPos;
                previewObjects[i].SetActive(true);
            }
            else
            {
                // Créer un nouvel objet de préview
                GameObject previewObj = CreateHexagonPreview(hexWorldPos);
                if (previewObj != null)
                {
                    if (i < previewObjects.Count)
                    {
                        previewObjects[i] = previewObj;
                    }
                    else
                    {
                        previewObjects.Add(previewObj);
                    }
                }
            }
        }
        
        // Désactiver les objets non utilisés
        for (int i = brushArea.Length; i < previewObjects.Count; i++)
        {
            if (previewObjects[i] != null)
            {
                previewObjects[i].SetActive(false);
            }
        }
    }
    
    private GameObject CreateHexagonPreview(Vector3 position)
    {
        // Obtenir le prefab actuel depuis le TileSelector
        GameObject tilePrefab = null;
        if (tileSelector != null)
        {
            tilePrefab = tileSelector.GetCurrentTilePrefab();
        }
        
        if (tilePrefab == null)
        {
            Debug.LogError("Cannot create preview: no tile prefab selected!");
            return null;
        }
        
        // Instancier le prefab
        GameObject hexObj = Instantiate(tilePrefab, position, Quaternion.identity);
        hexObj.name = "HexPreview";
        hexObj.transform.SetParent(transform);
        
        // Désactiver les colliders pour éviter les interactions avec le raycast
        Collider[] colliders = hexObj.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }
        
        // Appliquer le matériau de préview
        if (previewMaterialTemplate != null)
        {
            // Créer une instance du matériau pour cette preview
            if (previewMaterial == null)
            {
                previewMaterial = new Material(previewMaterialTemplate);
                previewMaterial.color = previewColor;
            }
            
            // Appliquer le matériau à tous les renderers
            Renderer[] renderers = hexObj.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                // Remplacer tous les matériaux par le matériau de préview
                Material[] newMaterials = new Material[rend.materials.Length];
                for (int i = 0; i < newMaterials.Length; i++)
                {
                    newMaterials[i] = previewMaterial;
                }
                rend.materials = newMaterials;
                rend.enabled = true;
            }
        }
        else
        {
            Debug.LogWarning("PreviewMaterialTemplate is not assigned! Using original materials.");
        }
        
        return hexObj;
    }
    
    private void ClearPreview()
    {
        foreach (GameObject obj in previewObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }
        lastCenterHex = new Vector2Int(int.MinValue, int.MinValue);
    }
    
    // Convertir une position world en coordonnées axiales hexagonales (flat-top)
    private Vector2Int WorldToHexAxial(Vector3 worldPosition)
    {
        float x = worldPosition.x;
        float z = worldPosition.z;
        
        float sizeInternal = hexSize / 2f;
        
        float q = (2f / 3f * x) / sizeInternal;
        float r = (-1f / 3f * x + Mathf.Sqrt(3f) / 3f * z) / sizeInternal;
        
        return HexRound(q, r);
    }
    
    // Arrondir les coordonnées fractionnelles vers les coordonnées hexagonales entières
    private Vector2Int HexRound(float q, float r)
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
        
        return new Vector2Int(roundedQ, roundedR);
    }
    
    // Convertir des coordonnées axiales hexagonales en position world
    private Vector3 HexAxialToWorld(int q, int r)
    {
        float sizeInternal = hexSize / 2f;
        
        float x = sizeInternal * (3f / 2f * q);
        float z = sizeInternal * (Mathf.Sqrt(3f) / 2f * q + Mathf.Sqrt(3f) * r);
        
        return new Vector3(x, 0f, z);
    }
    
    // Méthode publique pour activer/désactiver la préview
    public void SetPreviewActive(bool active)
    {
        showPreview = active;
        if (!active)
        {
            ClearPreview();
        }
    }
    
    // Méthode publique pour changer la couleur de la préview
    public void SetPreviewColor(Color color)
    {
        previewColor = color;
        if (previewMaterial != null)
        {
            previewMaterial.color = color;
        }
    }
    
    private void OnDestroy()
    {
        // Nettoyer les objets de préview
        foreach (GameObject obj in previewObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
        
        if (previewMaterial != null)
            Destroy(previewMaterial);
    }
}

