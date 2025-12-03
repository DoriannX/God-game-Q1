using UnityEngine;

public class BrushSizeManager : MonoBehaviour
{
    [Header("Brush Size Settings")]
    [SerializeField] private int minBrushSize = 1;
    [SerializeField] private int maxBrushSize = 10;
    [SerializeField] private int defaultBrushSize = 1;
    [SerializeField] private float scrollSensitivity = 1f; // Sensibilité de la molette
    
    private int currentBrushSize;
    
    // Event pour notifier les autres scripts quand la taille change
    public delegate void BrushSizeChangedDelegate(int newSize);
    public event BrushSizeChangedDelegate OnBrushSizeChanged;
    
    private void Start()
    {
        currentBrushSize = defaultBrushSize;
    }
    
    private void Update()
    {
        // Détecter Ctrl + Molette de souris
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            float scroll = Input.mouseScrollDelta.y;
            
            if (scroll != 0)
            {
                ChangeBrushSize(scroll);
            }
        }
    }
    
    private void ChangeBrushSize(float scrollDelta)
    {
        int oldSize = currentBrushSize;
        
        // Augmenter ou diminuer la taille en fonction du scroll
        if (scrollDelta > 0)
        {
            currentBrushSize = Mathf.Min(currentBrushSize + Mathf.CeilToInt(scrollDelta * scrollSensitivity), maxBrushSize);
        }
        else if (scrollDelta < 0)
        {
            currentBrushSize = Mathf.Max(currentBrushSize + Mathf.FloorToInt(scrollDelta * scrollSensitivity), minBrushSize);
        }
        
        // Si la taille a changé, notifier les listeners
        if (oldSize != currentBrushSize)
        {
            OnBrushSizeChanged?.Invoke(currentBrushSize);
        }
    }
    
    // Méthode publique pour obtenir la taille actuelle du brush
    public int GetBrushSize()
    {
        return currentBrushSize;
    }
    
    // Méthode publique pour définir manuellement la taille du brush
    public void SetBrushSize(int size)
    {
        int clampedSize = Mathf.Clamp(size, minBrushSize, maxBrushSize);
        
        if (clampedSize != currentBrushSize)
        {
            int oldSize = currentBrushSize;
            currentBrushSize = clampedSize;
            OnBrushSizeChanged?.Invoke(currentBrushSize);
        }
    }
    
    // Méthode pour obtenir les coordonnées hexagonales dans la zone du brush
    // Retourne une liste de coordonnées hexagonales en fonction de la taille du brush
    public Vector2Int[] GetBrushArea(Vector2Int centerHex)
    {
        if (currentBrushSize == 1)
        {
            // Brush de taille 1 = seulement le centre
            return new Vector2Int[] { centerHex };
        }
        
        // Calculer tous les hexagones dans le rayon du brush
        int radius = currentBrushSize - 1;
        System.Collections.Generic.List<Vector2Int> hexagons = new System.Collections.Generic.List<Vector2Int>();
        
        // Algorithme pour obtenir tous les hexagones dans un rayon (coordonnées axiales)
        for (int q = -radius; q <= radius; q++)
        {
            int r1 = Mathf.Max(-radius, -q - radius);
            int r2 = Mathf.Min(radius, -q + radius);
            
            for (int r = r1; r <= r2; r++)
            {
                hexagons.Add(new Vector2Int(centerHex.x + q, centerHex.y + r));
            }
        }
        
        return hexagons.ToArray();
    }
    
    // Visualisation de la zone du brush dans l'éditeur
    private void OnGUI()
    {
        // Afficher la taille du brush en haut à gauche
        GUI.Label(new Rect(10, 10, 200, 30), $"Brush Size: {currentBrushSize}", 
            new GUIStyle()
            {
                fontSize = 20,
                normal = new GUIStyleState() { textColor = Color.white }
            });
        
        // Afficher les instructions
        GUI.Label(new Rect(10, 40, 300, 20), "Ctrl + Scroll to change brush size", 
            new GUIStyle()
            {
                fontSize = 14,
                normal = new GUIStyleState() { textColor = Color.gray }
            });
    }
}

