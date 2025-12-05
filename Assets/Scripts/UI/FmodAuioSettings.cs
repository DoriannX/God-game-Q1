using UnityEngine;
using UnityEngine.UI;

public class FmodAudioSettings : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    FMOD.Studio.Bus masterBus;
    FMOD.Studio.Bus musicBus;
    FMOD.Studio.Bus sfxBus;

    const string MASTER_KEY = "MasterVolume";
    const string MUSIC_KEY = "MusicVolume";
    const string SFX_KEY = "SFXVolume";

    private void Awake()
    {
        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
        musicBus = FMODUnity.RuntimeManager.GetBus("bus:/Music");
        sfxBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSfxVolume);
    }

    private void Start()
    {
        // Charger les valeurs sauvegardées (1 par défaut)
        masterSlider.value = PlayerPrefs.GetFloat(MASTER_KEY, 1f);
        musicSlider.value = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        sfxSlider.value = PlayerPrefs.GetFloat(SFX_KEY, 1f);

        // Appliquer aux bus
        SetMasterVolume(masterSlider.value);
        SetMusicVolume(musicSlider.value);
        SetSfxVolume(sfxSlider.value);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }

    void SetMasterVolume(float value)
    {
        masterBus.setVolume(value);
        PlayerPrefs.SetFloat(MASTER_KEY, value);
    }

    void SetMusicVolume(float value)
    {
        musicBus.setVolume(value);
        PlayerPrefs.SetFloat(MUSIC_KEY, value);
    }

    void SetSfxVolume(float value)
    {
        sfxBus.setVolume(value);
        PlayerPrefs.SetFloat(SFX_KEY, value);
    }
}
