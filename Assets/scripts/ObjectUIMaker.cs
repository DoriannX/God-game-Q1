using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectUIMaker : MonoBehaviour
{
    [SerializeField] private ObjectPoserComponent poserComponent;
    [SerializeField] private Image prefabImage;
    [SerializeField] private Transform parentTransform;
    private Selector selector;
    private List<Image> images = new();

    private void Awake()
    {
        selector = GetComponentInChildren<Selector>();
        for(var i = 0; i < poserComponent.posableObjects.Length; i++)
        {
            Image img = Instantiate(prefabImage, parentTransform);
            images.Add(img);
            img.sprite = poserComponent.posableObjects[i].sprite;
            int index = i;
            img.GetComponent<Button>().onClick.AddListener(() =>
            {
                poserComponent.SetCurrentObject(index);
                selector.Select(img);
            });
        }
        images[0].GetComponent<Button>().onClick?.Invoke();
    }
}