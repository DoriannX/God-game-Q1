using UnityEngine;

/// <summary>
/// Affiche les FPS en haut au milieu de l'écran
/// </summary>
public class FPSCounter : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool showFPS = true;
    [SerializeField] private float updateInterval = 0.5f; // Mise à jour tous les 0.5 secondes
    
    [Header("Display")]
    [SerializeField] private int fontSize = 20;
    [SerializeField] private Color goodFPSColor = Color.green; // >= 50 FPS
    [SerializeField] private Color mediumFPSColor = Color.yellow; // >= 30 FPS
    [SerializeField] private Color badFPSColor = Color.red; // < 30 FPS
    
    private float fps = 0f;
    private float accum = 0f;
    private int frames = 0;
    private float timeLeft;
    
    private GUIStyle style;
    
    private void Start()
    {
        timeLeft = updateInterval;
        
        // Créer le style pour le texte
        style = new GUIStyle();
        style.fontSize = fontSize;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.UpperCenter;
        style.normal.textColor = Color.white;
    }
    
    private void Update()
    {
        if (!showFPS)
            return;
        
        timeLeft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        frames++;
        
        // Mettre à jour le FPS affiché
        if (timeLeft <= 0.0f)
        {
            fps = accum / frames;
            timeLeft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }
    
    private void OnGUI()
    {
        if (!showFPS)
            return;
        
        // Déterminer la couleur en fonction du FPS
        if (fps >= 50f)
            style.normal.textColor = goodFPSColor;
        else if (fps >= 30f)
            style.normal.textColor = mediumFPSColor;
        else
            style.normal.textColor = badFPSColor;
        
        // Position en haut au milieu
        float width = 200f;
        float height = 30f;
        float x = (Screen.width - width) / 2f;
        float y = 10f;
        
        // Afficher les FPS
        GUI.Label(new Rect(x, y, width, height), $"FPS: {fps:F0}", style);
    }
    
    /// <summary>
    /// Active/désactive l'affichage des FPS
    /// </summary>
    public void SetFPSVisible(bool visible)
    {
        showFPS = visible;
    }
}

