using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeteoManager : MonoBehaviour
{
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
}
