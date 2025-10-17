using System;
using SaveLoadSystem;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(SaveableEntity))]
public class DayCycleSystem : MonoBehaviour, ISaveable
{
    [Serializable]
    public struct DayCycleData
    {
        public bool isDay;
        public float timeElapsed;
    }
    public static DayCycleSystem instance;
    [SerializeField] private Light2D globalLight;

    [field: SerializeField]
    [field: Range(0f, 1f)]
    public float minIntensity { get; private set; } = 0.2f;

    [field: SerializeField]
    [field: Range(0f, 1f)]
    public float maxIntensity { get; private set; } = 0.8f;

    [SerializeField] private float dayDuration = 60f; // Duration of a full day in seconds
    public float currentIntensityPercent => Mathf.InverseLerp(minIntensity, maxIntensity, globalLight.intensity);
    private float timeElapsed = 0f;
    private bool isDay = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void OnEnable()
    {
        TickSystem.ticked += OnTicked;
    }

    private void Start()
    {
        SetMidDay();
    }

    private void OnTicked()
    {
        UpdateLight();
    }

    private void UpdateLight()
    {
        timeElapsed += 1f;
        float halfDayTicks = dayDuration / 2f;
    
        float t = timeElapsed / halfDayTicks;
        t = Mathf.Clamp01(t);
    
        if (isDay)
        {
            globalLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
            if (!(timeElapsed >= halfDayTicks))
            {
                return;
            }
            isDay = false;
        }
        else
        {
            globalLight.intensity = Mathf.Lerp(maxIntensity, minIntensity, t);
            if (!(timeElapsed >= halfDayTicks))
            {
                return;
            }
            isDay = true;
        }

        timeElapsed = 0f;
    }

    public void SetMidDay()
    {
        isDay = true;
        timeElapsed = dayDuration / 2f;
        UpdateLight();
    }
    
    public void SetMidNight()
    {
        isDay = false;
        timeElapsed = dayDuration / 2f;
        UpdateLight();
    }

    private void OnDisable()
    {
        
        TickSystem.ticked -= OnTicked;
    }

    public bool NeedsToBeSaved() => true;

    public bool NeedsReinstantiation() => false;

    public object SaveState()
    {
        DayCycleData data = new DayCycleData
        {
            isDay = isDay,
            timeElapsed = timeElapsed
        };
        return data;
    }

    public void LoadState(object state)
    {
        DayCycleData data = (DayCycleData)state;
        isDay = data.isDay;
        timeElapsed = data.timeElapsed;
        UpdateLight();
    }

    public void PostInstantiation(object state)
    {
    }

    public void GotAddedAsChild(GameObject obj, GameObject hisParent)
    {
    }
}
