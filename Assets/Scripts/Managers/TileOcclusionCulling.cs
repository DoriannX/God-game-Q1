// ============================================================================
// SCRIPT DÉSACTIVÉ - CONSERVÉ POUR USAGE FUTUR
// ============================================================================
// Ce script implémente un système d'occlusion culling pour optimiser le rendu des tiles.
// Il est actuellement désactivé mais conservé pour une utilisation future si nécessaire.
// 
// Pour l'utiliser :
// 1. Attacher ce script à un GameObject dans la scène
// 2. Assigner la caméra cible
// 3. Configurer les paramètres de culling selon vos besoins
// 4. Appeler RegisterTile() pour chaque tile créée
// 5. Appeler UnregisterTile() pour chaque tile détruite
// ============================================================================

using UnityEngine;
using System.Collections.Generic;

public class TileOcclusionCulling : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float updateInterval = 0.2f; // Mise à jour 5 fois par seconde
    [SerializeField] private float cullingDistance = 50f; // Distance max de rendu
    [SerializeField] private bool enableDistanceCulling = true;
    [SerializeField] private bool enableOcclusionCulling = true;
    [SerializeField] private LayerMask occlusionLayers = -1; // Quels layers peuvent occlure
    [SerializeField] private float raycastOffset = 0.5f; // Offset pour éviter les self-collisions
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    [SerializeField] private bool showDebugRays = false;
    
    private float lastUpdateTime;
    private Dictionary<GameObject, Renderer[]> tileRenderers = new Dictionary<GameObject, Renderer[]>();
    private int visibleCount;
    private int totalCount;
    
    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
        
        if (targetCamera == null)
        {
            Debug.LogError("TileOcclusionCulling: No camera found!");
            enabled = false;
        }
    }
    
    private void Update()
    {
        // Mise à jour à intervalle régulier pour éviter de surcharger
        if (Time.time >= lastUpdateTime + updateInterval)
        {
            UpdateVisibility();
            lastUpdateTime = Time.time;
        }
    }
    
    // Enregistrer une tile pour le culling
    public void RegisterTile(GameObject tile)
    {
        if (tile == null || tileRenderers.ContainsKey(tile))
            return;
        
        // Récupérer tous les renderers de la tile
        Renderer[] renderers = tile.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            tileRenderers[tile] = renderers;
        }
    }
    
    // Désenregistrer une tile
    public void UnregisterTile(GameObject tile)
    {
        if (tile != null && tileRenderers.ContainsKey(tile))
        {
            tileRenderers.Remove(tile);
        }
    }
    
    // Mettre à jour la visibilité de toutes les tiles
    private void UpdateVisibility()
    {
        if (targetCamera == null)
            return;
        
        visibleCount = 0;
        totalCount = tileRenderers.Count;
        
        Vector3 cameraPos = targetCamera.transform.position;
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(targetCamera);
        
        foreach (var kvp in tileRenderers)
        {
            GameObject tile = kvp.Key;
            Renderer[] renderers = kvp.Value;
            
            // Vérifier si la tile existe toujours
            if (tile == null || renderers == null || renderers.Length == 0)
                continue;
            
            bool isVisible = true;
            Vector3 tilePos = tile.transform.position;
            
            // Test 1: Frustum culling (vérifier si dans le champ de vision)
            bool inFrustum = false;
            foreach (Renderer rend in renderers)
            {
                if (rend != null && GeometryUtility.TestPlanesAABB(frustumPlanes, rend.bounds))
                {
                    inFrustum = true;
                    break;
                }
            }
            
            if (!inFrustum)
            {
                isVisible = false;
            }
            
            // Test 2: Distance culling
            if (isVisible && enableDistanceCulling)
            {
                float distance = Vector3.Distance(cameraPos, tilePos);
                if (distance > cullingDistance)
                {
                    isVisible = false;
                }
            }
            
            // Test 3: Occlusion culling (vérifier si bloqué par d'autres objets)
            if (isVisible && enableOcclusionCulling)
            {
                Vector3 direction = tilePos - cameraPos;
                float distance = direction.magnitude;
                
                // Point de départ légèrement devant la caméra pour éviter les collisions avec elle-même
                Vector3 rayStart = cameraPos + direction.normalized * raycastOffset;
                float rayDistance = distance - raycastOffset;
                
                // Faire un raycast vers la tile
                RaycastHit hit;
                if (Physics.Raycast(rayStart, direction.normalized, out hit, rayDistance, occlusionLayers))
                {
                    // Si le raycast touche quelque chose avant la tile, elle est occludée
                    if (hit.collider.gameObject != tile)
                    {
                        // Vérifier si ce n'est pas un enfant de la tile
                        Transform parent = hit.transform;
                        bool isPartOfTile = false;
                        while (parent != null)
                        {
                            if (parent.gameObject == tile)
                            {
                                isPartOfTile = true;
                                break;
                            }
                            parent = parent.parent;
                        }
                        
                        if (!isPartOfTile)
                        {
                            isVisible = false;
                            
                            if (showDebugRays)
                            {
                                Debug.DrawLine(rayStart, hit.point, Color.red, updateInterval);
                            }
                        }
                    }
                }
                else if (showDebugRays && isVisible)
                {
                    Debug.DrawLine(rayStart, tilePos, Color.green, updateInterval);
                }
            }
            
            // Activer/désactiver les renderers
            foreach (Renderer rend in renderers)
            {
                if (rend != null && rend.enabled != isVisible)
                {
                    rend.enabled = isVisible;
                }
            }
            
            if (isVisible)
            {
                visibleCount++;
            }
        }
    }
    
    // Forcer une mise à jour immédiate
    public void ForceUpdate()
    {
        UpdateVisibility();
    }
    
    // Nettoyer les tiles nulles du dictionnaire
    public void CleanupNullTiles()
    {
        List<GameObject> toRemove = new List<GameObject>();
        
        foreach (var kvp in tileRenderers)
        {
            if (kvp.Key == null)
            {
                toRemove.Add(kvp.Key);
            }
        }
        
        foreach (GameObject tile in toRemove)
        {
            tileRenderers.Remove(tile);
        }
    }
    
    // Afficher les stats dans l'éditeur
    private void OnGUI()
    {
        if (showDebugInfo)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 14;
            style.alignment = TextAnchor.UpperRight;
            
            // Position en haut à droite de l'écran
            float width = 300f;
            float lineHeight = 25f;
            float x = Screen.width - width - 10f;
            float y = 10f;
            
            GUI.Label(new Rect(x, y, width, lineHeight), $"Tiles Visible: {visibleCount} / {totalCount}", style);
            GUI.Label(new Rect(x, y + lineHeight, width, lineHeight), $"Culled: {totalCount - visibleCount} ({(totalCount > 0 ? (100f * (totalCount - visibleCount) / totalCount) : 0):F1}%)", style);
        }
    }
    
    // Getter pour les stats
    public int GetVisibleCount() => visibleCount;
    public int GetTotalCount() => totalCount;
    public int GetCulledCount() => totalCount - visibleCount;
}

