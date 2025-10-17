using System;
using System.Collections.Generic;
using SaveLoadSystem;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SaveableEntity))]
public class MeteoManager : MonoBehaviour, ISaveable
{
    [Serializable]
    public struct MeteoData
    {
        public bool isRaining;
        public int manualTick;
        public bool setManually;
        public float rainIntensity;
    }
    public static MeteoManager Instance { get; private set; }
    [SerializeField] private RainController rainController;
    [SerializeField, Range(0, 1)] private float rainChance;
    [SerializeField, Range(0, 1)] private float sunChance;
    [SerializeField, Range(0, 1)] private float rainChangeChance;
    [SerializeField] private int ticksToAutomateWeather = 5;
    private int manualTick = 0;
    public bool isRaining { get; private set; }
    private bool setManually;
    public event Action<bool> weatherChanged;
    private readonly List<GameObject> activeWeatherEffects = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    public void SetWeather(bool raining)
    {
        setManually = true;
        manualTick = 0;
        if (raining)
        {
            StartRain();
        }
        else
        {
            StopRain();
        }
    }

    private void OnEnable()
    {
        TickSystem.ticked += OnTicked;
    }
    
    private void StartRain()
    {
        isRaining = true;
        rainController.masterIntensity = Random.Range(0.3f, 1f);
        weatherChanged?.Invoke(true);
    }
    
    private void ChangeRainIntensity()
    {
        rainController.masterIntensity = Random.Range(0.3f, 1f);
    }
    
    private void StopRain()
    {
        isRaining = false;
        rainController.masterIntensity = 0f;
        weatherChanged?.Invoke(false);
    }
    
    private void OnTicked()
    {
        if (setManually)
        {
            manualTick++;
            if (manualTick >= ticksToAutomateWeather)
            {
                setManually = false;
                manualTick = 0;
            }
            return;
        }
        if (!isRaining)
        {
            if(Random.value < rainChance)
            {
                StartRain();
            }
        }
        else 
        {
            if(rainChangeChance > Random.value)
            {
                ChangeRainIntensity();
                return;
            }
            if(Random.value < sunChance)
            {
                StopRain();
            }
        }
    }
    
    private void OnDisable()
    {
        TickSystem.ticked -= OnTicked;
    }

    public bool NeedsToBeSaved()
    {
        return true;
    }

    public bool NeedsReinstantiation()
    {
        return false;
    }

    public object SaveState()
    {
        MeteoData data = new MeteoData
        {
            isRaining = isRaining,
            manualTick = manualTick,
            setManually = setManually,
            rainIntensity = rainController.masterIntensity
        };
        return data;
    }

    public void LoadState(object state)
    {
        MeteoData data = (MeteoData)state;
        isRaining = data.isRaining;
        manualTick = data.manualTick;
        setManually = data.setManually;
        rainController.masterIntensity = isRaining ? data.rainIntensity : 0f;
        weatherChanged?.Invoke(isRaining);
    }

    public void PostInstantiation(object state)
    {
    }

    public void GotAddedAsChild(GameObject obj, GameObject hisParent)
    {
    }
}
