using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class DestructionObject : MonoBehaviour
{
    public abstract void Destroy(Tilemap tilemap, WaterSystem waterSystem, HeightManager heightManager, Vector2 pos);
    [field: SerializeField] public Sprite sprite { get; private set; }
}