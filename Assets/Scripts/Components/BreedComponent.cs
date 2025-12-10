using System;
using UnityEngine;

public class BreedComponent : MonoBehaviour {
    public bool isBreedDone { get; private set; }
    public event Action onBreedDone;

    private void Awake()
    {
        onBreedDone += () => isBreedDone = true;
    }
    
}
