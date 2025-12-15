using System;
using UnityEngine;

[Serializable]
public class MeteoEvent : MonoBehaviour
{
    [SerializeField] protected MeteoState state;
    [SerializeField, Range(0, 100)] protected float chance;

    public MeteoState GetState()
    {
        return state;
    }

    public float GetChance()
    {
        return chance;
    }
}
