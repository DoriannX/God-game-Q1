using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PosableObject : MonoBehaviour
{
   private Sprite cachedSprite;

   public Sprite sprite
   {
      get
      {
         if (cachedSprite == null)
            cachedSprite = GetComponent<SpriteRenderer>().sprite;
         return cachedSprite;
      }
   }


   [field:SerializeField] public List<TileBase> allowedTiles { get; private set; } = new();
}
