using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestructionUIMaker : MonoBehaviour
{
    [SerializeField] private DestructionComponent destructionComponent;
    [SerializeField] private Painter painter;
    [SerializeField] private Image prefabImage;
    [SerializeField] private RectTransform parentTransform;
    private Selector selector;
    private List<Image> images = new();

    private void Awake()
    {
        selector = GetComponentInChildren<Selector>();
        for (int i = 0; i < destructionComponent.destructionObjects.Count; i++)
        {
            Image img = Instantiate(prefabImage, parentTransform);
            images.Add(img);
            img.sprite = destructionComponent.destructionObjects[i].sprite;
            int index = i;
            img.GetComponent<Button>().onClick.AddListener(() =>
            {
                destructionComponent.SetCurrentObject(index);
                selector.Select(img);
            });
        }
        images[0].GetComponent<Button>().onClick?.Invoke();
    } 
}