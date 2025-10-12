using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaletteUIMaker : MonoBehaviour
{
    [SerializeField] private PaintComponent paintComponent;
    [SerializeField] private Painter painter;
    [SerializeField] private Image prefabImage;
    private Selector selector;
    private List<Image> images = new();

    private void Awake()
    {
        selector = GetComponentInChildren<Selector>();
        for(int i = 0; i < paintComponent.tiles.Length; i++)
        {
            Image img = Instantiate(prefabImage, transform);
            images.Add(img);
            img.sprite = paintComponent.tiles[i].sprite;
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