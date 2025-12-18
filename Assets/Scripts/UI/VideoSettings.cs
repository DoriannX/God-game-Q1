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
        if (resolutionDropdown == null)
        {
            Debug.LogWarning("resolutionDropdown is null");
            return;
        }

        if (fullscreenToggle == null)
            Debug.LogWarning("VideoSettings is null");

        Resolution[] allResolutions = Screen.resolutions;

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

        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        currentResolutionIndex = 0;

        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            Resolution res = filteredResolutions[i];
            string option = res.width + " x " + res.height;
            options.Add(option);

            if (res.width == Screen.currentResolution.width &&
                res.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = Mathf.Clamp(currentResolutionIndex, 0, Mathf.Max(0, filteredResolutions.Count - 1));
        resolutionDropdown.RefreshShownValue();

        if (fullscreenToggle != null)
            fullscreenToggle.isOn = Screen.fullScreen;
    }

    public void OnResolutionChanged(int dropdownIndex)
    {
        if (filteredResolutions == null || filteredResolutions.Count == 0)
        {
            Debug.LogWarning("0 resolution");
            return;
        }

        if (dropdownIndex < 0 || dropdownIndex >= filteredResolutions.Count)
        {
            Debug.LogWarning($"VideoSettings: index de dropdown invalide ({dropdownIndex}).");
            return;
        }

        Resolution res = filteredResolutions[dropdownIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void OnFullscreenToggled(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        if (filteredResolutions != null && filteredResolutions.Count > 0 && resolutionDropdown != null)
        {
            int idx = Mathf.Clamp(resolutionDropdown.value, 0, filteredResolutions.Count - 1);
            Resolution res = filteredResolutions[idx];
            Screen.SetResolution(res.width, res.height, isFullscreen);
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
