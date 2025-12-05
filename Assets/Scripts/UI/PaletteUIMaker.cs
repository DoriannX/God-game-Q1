using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaletteUIMaker : MonoBehaviour
{
    [SerializeField] private Painter painter;
    [SerializeField] private PaintComponent paintComponent;
    [SerializeField] private Image prefabImage;
    private Selector selector;
    private List<Image> images = new();

    private void Awake()
    {
        selector = GetComponentInChildren<Selector>();
        for(int i = 0; i < TileSelector.instance.AvailableTiles.Length; i++)
        {
            Image img = Instantiate(prefabImage, transform);
            images.Add(img);
            img.sprite = TileSelector.instance.AvailableTiles[i].tileIcon;
            int index = i;
            img.GetComponent<Button>().onClick.AddListener(() =>
            {
                paintComponent.SetCurrentTile(index);
                selector.Select(img);
            });
        }
        images[0].GetComponent<Button>().onClick?.Invoke();
    }
}