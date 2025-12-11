using System.Collections.Generic;
using UnityEngine;

public abstract class Posable : MonoBehaviour
{
    private Sprite cachedSprite;

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

    [field: SerializeField] public List<GameObject> allowedTiles { get; private set; } = new();
}

