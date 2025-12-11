using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class DestructionObject : Posable
{
    public abstract void Destroy();
    [field: SerializeField] public Sprite sprite { get; private set; }
}