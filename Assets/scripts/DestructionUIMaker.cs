using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestructionUIMaker : MonoBehaviour
{
    [SerializeField] private DestructionComponent destructionComponent;
    [SerializeField] private Painter painter;
    [SerializeField] private Image prefabImage;
    [SerializeField] private RectTransform parentTransform;
    [SerializeField] private Image selectedBorder;
    private List<Image> images = new();

    private void Awake()
    {
        for(int i = 0; i < destructionComponent.destructionObjects.Count; i++)
        {
            Image img = Instantiate(prefabImage, parentTransform);
            images.Add(img);
            img.sprite = destructionComponent.destructionObjects[i].sprite;
            int index = i;
            img.GetComponent<Button>().onClick.AddListener(() =>
            {
                destructionComponent.SetCurrentObject(index);
                selectedBorder.enabled = true;
                selectedBorder.transform.position = img.transform.position;
            });
        }
    }

    private void Update()
    {
        selectedBorder.transform.position = images[destructionComponent.objectIndex].transform.position;
    }
}