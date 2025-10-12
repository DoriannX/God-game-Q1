using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class Selector : MonoBehaviour
{
    private RectTransform rectTransform;
    private GameObject selectedObject;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Select(MonoBehaviour obj)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        selectedObject = obj.gameObject;
    }
    
    private void GoToSelected()
    {
        if(selectedObject == null)
        {
            return;
        }
        if(selectedObject.TryGetComponent(out RectTransform objRectTransform))
        {
            rectTransform.sizeDelta = objRectTransform.sizeDelta;
            rectTransform.localPosition = objRectTransform.localPosition;
        }
        else
        {
            rectTransform.localPosition = selectedObject.transform.position;
        }
    }

    private void Update()
    {
        if(selectedObject != null)
        {
            GoToSelected();
        }
    }
}
