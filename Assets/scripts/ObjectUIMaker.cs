using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectUIMaker : MonoBehaviour
{
    [SerializeField] private ObjectPoserComponent poserComponent;
    [SerializeField] private Image prefabImage;
    [SerializeField] private Transform parentTransform;
    [SerializeField] private Image selectedBorder;
    private List<Image> images = new();

    private void Awake()
    {
        
        for(var i = 0; i < poserComponent.posableObjects.Length; i++)
        {
            Image img = Instantiate(prefabImage, parentTransform);
            images.Add(img);
            img.sprite = poserComponent.posableObjects[i].sprite;
            int index = i;
            img.GetComponent<Button>().onClick.AddListener(() =>
            {
                poserComponent.SetCurrentObject(index);
                selectedBorder.enabled = true;
                selectedBorder.transform.position = img.transform.position;
            });
        }
    }
    
    private void Update()
    {
        selectedBorder.transform.position = images[poserComponent.objectIndex].transform.position;
    }
}