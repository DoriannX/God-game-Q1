using UnityEngine;

public class Rain : MeteoEvent
{
    [SerializeField] private RainController controller;
    [SerializeField, Range(0, 1)] private float rainChangeChance;

    private void OnEnable()
    {
        controller.masterIntensity = Random.Range(0.3f, 1f);
        TickSystem.ticked += OnTicked;
    }

    private void OnTicked()
    {
        if (rainChangeChance > Random.value)
        {
            controller.masterIntensity = Random.Range(0.3f, 1f);
            return;
        }
    }

    private void OnDisable()
    {
        controller.masterIntensity = 0.0f;
        TickSystem.ticked -= OnTicked;
    }
}