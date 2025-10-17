using System;
using UnityEngine;
using UnityEngine.UI;

public class UISelector : MonoBehaviour
{
    [SerializeField] private Image leftArrow;
    [SerializeField] private Image rightArrow;

    private void Start()
    {
        Deselect();
    }


    public void Select(GameObject selectedObject)
    {
        gameObject.SetActive(true);
        RectTransform rectTransform = selectedObject.GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        
        leftArrow.transform.position = corners[1] + Vector3.left * (leftArrow.rectTransform.rect.width / 2) + Vector3.down * (leftArrow.rectTransform.rect.height / 2);
        rightArrow.transform.position = corners[2] + Vector3.right * (rightArrow.rectTransform.rect.width / 2) + Vector3.down * (rightArrow.rectTransform.rect.height / 2);
    }
    
    public void Deselect()
    {
        gameObject.SetActive(false);
    }
}
