using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaletteUIMaker : MonoBehaviour
{
    [SerializeField] private PaintComponent paintComponent;
    [SerializeField] private Painter painter;
    [SerializeField] private Image prefabImage;
    [SerializeField] private RectTransform parentTransform;
    [SerializeField] private Image selectedBorder;
    private List<Image> images = new();

    private void Awake()
    {
        for(int i = 0; i < paintComponent.tiles.Length; i++)
        {
            Image img = Instantiate(prefabImage, parentTransform);
            images.Add(img);
            img.sprite = paintComponent.tiles[i].sprite;
            int index = i;
            img.GetComponent<Button>().onClick.AddListener(() =>
            {
                paintComponent.SetCurrentTile(index);
                selectedBorder.enabled = true;
                selectedBorder.transform.position = img.transform.position;
            });
        }
    }

    private void Update()
    {
        selectedBorder.transform.position = images[paintComponent.tileIndex].transform.position;
    }
}