using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PosableObject : MonoBehaviour
{
    private Sprite cachedSprite;
    private GhostIa ghostIa;
    private House house;

    public Sprite sprite
    {
        get
        {
            if (cachedSprite == null)
            {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                    spriteRenderer = GetComponentInChildren<SpriteRenderer>();

                cachedSprite = spriteRenderer.sprite;
            }

            return cachedSprite;
        }
    }

    private void Awake()
    {
        ghostIa = GetComponent<GhostIa>();
        house = GetComponent<House>();
    }

    [field: SerializeField] public List<GameObject> allowedTiles { get; private set; } = new();
}