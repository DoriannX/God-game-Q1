using System;
using UnityEngine;

public class TickSystem : MonoBehaviour
{
    public int tick { get; private set; } = 0;
    public float tickInterval = 1f;
    private float tickTimer = 0f;
    public static event Action ticked; 

    private void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= tickInterval)
        {
            tickTimer -= tickInterval;
            tick++;
            OnTick();
        }
    }

    protected virtual void OnTick()
    {
        ticked?.Invoke();
    }

    private void OnDestroy()
    {
        ticked = null;
    }
}
