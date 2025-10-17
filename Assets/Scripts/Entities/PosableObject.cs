using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PosableObject : MonoBehaviour
{
    private Sprite cachedSprite;
    private GhostIa ghostIa;

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
    }

    private void OnEnable()
    {
        TickSystem.ticked += OnTick;
    }

    private void OnTick()
    {
        if (WaterSystem.instance.IsOnWater(transform.position))
        {
            if (ghostIa == null)
            {
                Destroy(gameObject);
            }
            else
            {
                GhostManager.instance.RemoveGhost(gameObject);
            }
        }
    }

    [field: SerializeField] public List<TileBase> allowedTiles { get; private set; } = new();

    private void OnDisable()
    {
        TickSystem.ticked -= OnTick;
    }
}