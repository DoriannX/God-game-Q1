using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayCycleSystem : MonoBehaviour
{
    [SerializeField] private Light2D globalLight;
    [SerializeField, Range(0f, 1f)] private float minIntensity = 0.2f;
    [SerializeField, Range(0f, 1f)] private float maxIntensity = 0.8f;
    [SerializeField] private float dayDuration = 60f; // Duration of a full day in seconds
    private float timeElapsed = 0f;
    private bool isDay = false;

    private void Awake()
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
}
