using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
                                                                                                        
public class VolumeSliderUI : MonoBehaviour
{
    [Header("Master")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private TextMeshProUGUI masterText;

    [Header("Music")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TextMeshProUGUI musicText;

    [Header("SFX")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxText;

    // Stocke les callbacks pour pouvoir les retirer proprement
    private UnityAction<float> masterListener;
    private UnityAction<float> musicListener;
    private UnityAction<float> sfxListener;

    private void Start()
    {
        // Définit et attache les listeners
        if (masterSlider != null)
        {
            masterListener = v => UpdateVolumeText(v, masterText);
            masterSlider.onValueChanged.AddListener(masterListener);
            UpdateVolumeText(masterSlider.value, masterText);
        }

        if (musicSlider != null)
        {
            musicListener = v => UpdateVolumeText(v, musicText);
            musicSlider.onValueChanged.AddListener(musicListener);
            UpdateVolumeText(musicSlider.value, musicText);
        }

        if (sfxSlider != null)
        {
            sfxListener = v => UpdateVolumeText(v, sfxText);
            sfxSlider.onValueChanged.AddListener(sfxListener);
            UpdateVolumeText(sfxSlider.value, sfxText);
        }
    }

    private void OnDestroy()
    {
        if (masterSlider != null && masterListener != null) masterSlider.onValueChanged.RemoveListener(masterListener);
        if (musicSlider != null && musicListener != null) musicSlider.onValueChanged.RemoveListener(musicListener);
        if (sfxSlider != null && sfxListener != null) sfxSlider.onValueChanged.RemoveListener(sfxListener);
    }

    private void UpdateVolumeText(float value, TextMeshProUGUI text)
    {
        if (text == null) return;
        int percent = Mathf.RoundToInt(value * 100f);
        text.text = percent + " %";
    }
}
