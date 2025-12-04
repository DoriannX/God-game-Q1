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

    private void OnEnable()
    {
        TickSystem.ticked += OnTick;
    }

    private void OnTick()
    {
        if (WaterSystem.instance.IsOnWater(transform.position))
        {
            if (ghostIa != null)
            {
                GhostManager.instance.RemoveGhost(gameObject);
            }
            else if (house != null)
            {
                GhostManager.instance.UnregisterGhostInHouse(house);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    [field: SerializeField] public List<GameObject> allowedTiles { get; private set; } = new();

    private void OnDisable()
    {
        TickSystem.ticked -= OnTick;
    }
}