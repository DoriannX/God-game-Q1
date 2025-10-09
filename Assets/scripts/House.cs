using UnityEngine;

public class House: WorkTask
{
    [SerializeField] private Sprite[] houseSprites;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public override void Work()
    {
        base.Work();
        int spriteIndex = Mathf.FloorToInt(progress * houseSprites.Length);
        spriteIndex = Mathf.Clamp(spriteIndex, 0, houseSprites.Length - 1);
        spriteRenderer.sprite = houseSprites[spriteIndex];
    }
    protected override void OnComplete(){}
}