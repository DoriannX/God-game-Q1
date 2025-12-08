using UnityEngine;
using System.Collections.Generic;

public class BrushPreview : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private BrushSizeManager brushManager;

    [SerializeField] private TileSelector tileSelector;
    [SerializeField] private Material previewMaterialTemplate;
    [SerializeField] private TilePool tilePool;

    [Header("Preview Settings")] [SerializeField]
    private Color previewColor = new Color(0f, 1f, 0f, 0.5f);

    [SerializeField] private float hexSize = 1f;
    [SerializeField] private float tileHeight = 0.2f;
    [SerializeField] private bool showPreview = true;

    private Camera mainCamera;
    private Material previewMaterial;
    private List<GameObject> previewObjects = new();
    private Vector3Int lastCenterHex = new(int.MinValue, int.MinValue, int.MinValue);
    private int lastBrushSize = -1;
    private int lastTileIndex = -1;
    private float updateInterval = 0.1f;
    private float lastUpdateTime;

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

        // Calculate intersection with Y = 0 plane
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        // Convert to hexagonal coordinates
        int currentBrushSize = brushManager.brushSize;
        int currentTileIndex = tileSelector != null ? tileSelector.currentTileIndex : -1;

        // Check if we need to update (position/size/tile changed OR interval elapsed)
        bool positionChanged = TilemapManager.instance.currentHexCoordinates != lastCenterHex || currentBrushSize != lastBrushSize;
        bool tileChanged = currentTileIndex != lastTileIndex;
        bool intervalElapsed = Time.time >= lastUpdateTime + updateInterval;

        if (positionChanged || tileChanged || intervalElapsed)
        {
            UpdatePreview(TilemapManager.instance.currentHexCoordinates);
            lastCenterHex = TilemapManager.instance.currentHexCoordinates;
            lastBrushSize = currentBrushSize;
            lastTileIndex = currentTileIndex;
            lastUpdateTime = Time.time;
        }
    }

    private void UpdatePreview(Vector3Int centerHex)
    {
        // Get brush area
        Vector2Int[] brushArea = brushManager.GetBrushArea(centerHex);

        // Get current tile index
        int currentTileIndex = tileSelector != null ? tileSelector.currentTileIndex : -1;

        // If tile changed, destroy all existing objects to recreate with new prefab
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

        // Reuse or create preview objects
        for (int i = 0; i < brushArea.Length; i++)
        {
            Vector2Int coord = brushArea[i];

            // Get column height using TilemapManager
            int height = TilemapManager.instance.GetColumnHeight(coord);

            // Calculate world position
            Vector3Int hexCoords = new Vector3Int(coord.x, coord.y, height + 1);
            Vector3 hexWorldPos = HexAxialToWorld(hexCoords);

            if (i < previewObjects.Count && previewObjects[i] != null)
            {
                // Reuse existing object
                previewObjects[i].transform.position = hexWorldPos;
                previewObjects[i].SetActive(true);
            }
            else
            {
                // Create new preview object
                GameObject previewObj = CreatePreviewObject(hexWorldPos);
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

        // Deactivate unused objects
        for (int i = brushArea.Length; i < previewObjects.Count; i++)
        {
            if (previewObjects[i] != null)
            {
                previewObjects[i].SetActive(false);
            }
        }
    }

    private GameObject CreatePreviewObject(Vector3 position)
    {

        // Instantiate the prefab
        GameObject previewObj = tilePool.GetTile();
        previewObj.name = "TilePreview";
        previewObj.transform.SetParent(transform);

        // Disable colliders to avoid raycast interactions
        Collider[] colliders = previewObj.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // Apply preview material
        if (previewMaterialTemplate != null)
        {
            // Create material instance if needed
            if (previewMaterial == null)
            {
                previewMaterial = new Material(previewMaterialTemplate);
                previewMaterial.color = previewColor;
            }

            // Apply material to all renderers
            Renderer[] renderers = previewObj.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                // Replace all materials with preview material
                Material[] newMaterials = new Material[rend.materials.Length];
                for (int i = 0; i < newMaterials.Length; i++)
                {
                    newMaterials[i] = previewMaterial;
                }

                rend.materials = newMaterials;
                rend.enabled = true;
            }
        }

        return previewObj;
    }

    private void ClearPreview()
    {
        foreach (GameObject obj in previewObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        lastCenterHex = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
    }

    // Convert world position to hexagonal axial coordinates (flat-top)
    private Vector3Int WorldToHexAxial(Vector3 worldPosition)
    {
        float x = worldPosition.x;
        float z = worldPosition.z;

        float sizeInternal = hexSize / 2f;

        float q = (2f / 3f * x) / sizeInternal;
        float r = (-1f / 3f * x + Mathf.Sqrt(3f) / 3f * z) / sizeInternal;

        return HexRound(q, r);
    }

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

        return new Vector3Int(roundedQ, roundedR, 0);
    }

    private Vector3 HexAxialToWorld(Vector3Int hexCoords)
    {
        float sizeInternal = hexSize / 2f;

        float x = sizeInternal * (3f / 2f * hexCoords.x);
        float z = sizeInternal * (Mathf.Sqrt(3f) / 2f * hexCoords.x + Mathf.Sqrt(3f) * hexCoords.y);

        return new Vector3(x, hexCoords.z * tileHeight, z);
    }

    public void SetPreviewActive(bool active)
    {
        showPreview = active;
        if (!active)
        {
            ClearPreview();
        }
    }

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
        if (!Application.isPlaying) return;

        // Clean up preview objects
        foreach (GameObject obj in previewObjects)
        {
            if (obj != null)
                Destroy(obj);
        }

        if (previewMaterial != null)
            Destroy(previewMaterial);
    }
}