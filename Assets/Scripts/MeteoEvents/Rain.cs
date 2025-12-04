using SaveLoadSystem;
using UnityEngine;

public class Rain : MeteoEvent, ISaveable
{
    public struct RainData
    {
        public float rainChangeChance;
        public float rainIntensity;
    }
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
        RainData data = new RainData
        {
            rainChangeChance = rainChangeChance,
            rainIntensity = controller.masterIntensity
        };
        return data;
    }

    public void LoadState(object state)
    {
        RainData data = (RainData)state;
        controller.masterIntensity = data.rainIntensity;
        rainChangeChance = data.rainChangeChance;
    }

    public void PostInstantiation(object state)
    {
    }

    public void GotAddedAsChild(GameObject obj, GameObject hisParent)
    {
    }
}
