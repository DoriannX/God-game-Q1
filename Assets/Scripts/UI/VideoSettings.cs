using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class VideoSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    private List<Resolution> filteredResolutions;
    private int currentResolutionIndex;

    private void Start()
    {
        // Sécurité : vérifier les références assignées
        if (resolutionDropdown == null)
        {
            Debug.LogWarning("VideoSettings: resolutionDropdown n'est pas assigné dans l'Inspector.");
            return;
        }

        if (fullscreenToggle == null)
            Debug.LogWarning("VideoSettings: fullscreenToggle n'est pas assigné dans l'Inspector.");

        // 1) Récupérer toutes les résolutions supportées par l'écran
        Resolution[] allResolutions = Screen.resolutions;

        // 2) Filtrer pour garder une seule entrée par (width, height)
        filteredResolutions = new List<Resolution>();
        HashSet<string> usedRes = new HashSet<string>();

        foreach (Resolution res in allResolutions)
        {
            string key = res.width + "x" + res.height;
            if (!usedRes.Contains(key))
            {
                usedRes.Add(key);
                filteredResolutions.Add(res);
            }
        }

        // 3) Remplir le dropdown
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        currentResolutionIndex = 0;

        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            Resolution res = filteredResolutions[i];
            string option = res.width + " x " + res.height;
            options.Add(option);

            // Trouver l'index correspondant à la résolution actuelle
            if (res.width == Screen.currentResolution.width &&
                res.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = Mathf.Clamp(currentResolutionIndex, 0, Mathf.Max(0, filteredResolutions.Count - 1));
        resolutionDropdown.RefreshShownValue();

        // 4) Initialiser le toggle fullscreen
        if (fullscreenToggle != null)
            fullscreenToggle.isOn = Screen.fullScreen;
    }

    // Appelé par le Dropdown (On Value Changed - int)
    public void OnResolutionChanged(int dropdownIndex)
    {
        if (filteredResolutions == null || filteredResolutions.Count == 0)
        {
            Debug.LogWarning("VideoSettings: aucune résolution filtrée disponible.");
            return;
        }

        if (dropdownIndex < 0 || dropdownIndex >= filteredResolutions.Count)
        {
            Debug.LogWarning($"VideoSettings: index de dropdown invalide ({dropdownIndex}).");
            return;
        }

        Resolution res = filteredResolutions[dropdownIndex];
        // On garde le mode plein écran actuel
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    // Appelé par le Toggle (On Value Changed - bool)
    public void OnFullscreenToggled(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        // Réappliquer la résolution sélectionnée avec le nouveau mode si possible
        if (filteredResolutions != null && filteredResolutions.Count > 0 && resolutionDropdown != null)
        {
            int idx = Mathf.Clamp(resolutionDropdown.value, 0, filteredResolutions.Count - 1);
            Resolution res = filteredResolutions[idx];
            Screen.SetResolution(res.width, res.height, isFullscreen);
        }
    }

    // Méthode publique pour revenir au menu principal (attacher au bouton Back dans la scène Options)
    public void BackToMainMenu()
    {
        // Charger par nom : s'assurer que "MainMenu" est présent dans File > Build Settings
        SceneManager.LoadScene("MainMenu");
    }
}
