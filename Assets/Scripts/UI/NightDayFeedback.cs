using System;
using UnityEngine;

public class NightDayFeedback : MonoBehaviour
{
    [SerializeField] private RectTransform content;
    [SerializeField] private float minX = -100f;
    [SerializeField] private float maxX = 100f;

    private void OnEnable()
    {
        TickSystem.ticked += OnTick;
    }

    private void OnTick()
    {
        float intensity = DayCycleSystem.instance.currentIntensityPercent;
        float t = Mathf.InverseLerp(DayCycleSystem.instance.minIntensity, DayCycleSystem.instance.maxIntensity, intensity);
        float xPos = Mathf.Lerp(minX, maxX, t);
        content.anchoredPosition = new Vector2(xPos, content.anchoredPosition.y);
    }

    private void OnDisable()
    {
        TickSystem.ticked -= OnTick;
    }
}
