using SaveLoadSystem;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

using FMODUnity;
using FMOD.Studio;


public enum MeteoState
{
    Neutral,
    Sunny,
    Raining,
    Tornado,
    Earthquake,
    Tsunami,
    Sandstorm
}

//[RequireComponent(typeof(SaveableEntity))]
public class MeteoManager : MonoBehaviour/*, ISaveable*/
{

    [EventRef]

    [SerializeField] private string rainEventPath = "event:/Ambiance/Rain";

    private EventInstance rainInstance;

    [Serializable]
    public struct MeteoData
    {
        public MeteoEvent activeMeteoObject;
        public int manualTick;
        public bool setManually;
        public MeteoState state;
    }
    public static MeteoManager Instance { get; private set; }
    [SerializeField, Range(0, 1)] private float rainChance;
    [SerializeField, Range(0, 1)] private float sunChance;
    [SerializeField] private List<MeteoEvent> meteoComponents = new List<MeteoEvent>();
    [SerializeField] private int ticksToAutomateWeather = 5;
    private int manualTick = 0;
    private bool setManually;
    public event Action<bool> weatherChanged;
    private readonly List<GameObject> activeWeatherEffects = new();
    private MeteoEvent activeMeteoEvent;
    public MeteoState state;
    float sum = 0;

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
        rainInstance = RuntimeManager.CreateInstance(rainEventPath);
    }

    public void SetWeather(MeteoState newState)
    {
        setManually = true;
        manualTick = 0;

        if (activeMeteoEvent != null && activeMeteoEvent.GetState() == newState)
        {
            return;
        }

        StopWeather();
        if (newState == MeteoState.Raining)
        {
            rainInstance.start();

        }
        StartWeather(newState);
    }

    private void OnEnable()
    {
        TickSystem.ticked += OnTicked;
    }

    private void StartWeather(MeteoState newState)
    {
        foreach (MeteoEvent meteoEvent in meteoComponents)
        {
            if (meteoEvent.GetState() == newState)
            {
                activeMeteoEvent = meteoEvent;
                activeMeteoEvent.gameObject.SetActive(true);
                state = newState;

                weatherChanged?.Invoke(state == MeteoState.Raining);
            }
        }
    }

    private void StopWeather()
    {
        if (activeMeteoEvent == null)
        {
            return;
        }
        activeMeteoEvent.gameObject.SetActive(false);
        activeMeteoEvent = null;

        rainInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        state = MeteoState.Sunny;
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

        //Algorithm to randomly select events with different chances of appearing.
        foreach (MeteoEvent component in meteoComponents)
        {
            sum += component.GetChance();
        }
        sum = Random.Range(0, sum);
        foreach (MeteoEvent component in meteoComponents)
        {
            sum -= component.GetChance();
            if (sum <= 0)
            {
                SetWeather(component.GetState());
                return;
            }
        }
    }

    private void OnDisable()
    {
        TickSystem.ticked -= OnTicked;
    }

    /*public bool NeedsToBeSaved()
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
            activeMeteoObject = activeMeteoEvent,
            manualTick = manualTick,
            setManually = setManually,
            state = state
        };
        return data;
    }

    public void LoadState(object state)
    {
        MeteoData data = (MeteoData)state;
        activeMeteoEvent = data.activeMeteoObject;
        manualTick = data.manualTick;
        setManually = data.setManually;
        state = data.state;
        weatherChanged?.Invoke(activeMeteoEvent);
    }

    public void PostInstantiation(object state)
    {
    }

    public void GotAddedAsChild(GameObject obj, GameObject hisParent)
    {
    }*/
    private void OnDestroy()
    {
        rainInstance.release(); // libère l’instance proprement [web:72]
    }


}
