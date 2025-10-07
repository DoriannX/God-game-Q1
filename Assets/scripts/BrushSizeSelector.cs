using System;
using UnityEngine;
using UnityEngine.UI;

public class BrushSizeSelector : MonoBehaviour
{
    [SerializeField] private Painter painter;
    [SerializeField] private float sizeOption1 = 0.1f;
    [SerializeField] private float sizeOption2 = 0.75f;
    [SerializeField] private float sizeOption3 = 1.5f;
    [SerializeField] private Image selectedIndicator;
    [SerializeField] private Button size1;
    [SerializeField] private Button size2;
    [SerializeField] private Button size3;

    private void Awake()
    {
        void SetupButton(Button button, float size)
        {
            button.onClick.AddListener(() =>
            {
                OnButtonClicked(size);
                selectedIndicator.transform.position = button.transform.position;
                selectedIndicator.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                    button.GetComponent<RectTransform>().rect.width);
                selectedIndicator.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                    button.GetComponent<RectTransform>().rect.height);
            });
        }
        
        SetupButton(size1, sizeOption1);
        SetupButton(size2, sizeOption2);
        SetupButton(size3, sizeOption3);
    }

    private void Start()
    {
        size1.onClick.Invoke();
    }

    private void OnButtonClicked(float size)
    {
        painter.SetBrushSize(size);
    }
}