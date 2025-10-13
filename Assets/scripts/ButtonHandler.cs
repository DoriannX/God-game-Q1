using System;
using TMPro;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    [SerializeField] private RectTransform textComponent;
    [SerializeField] private float moveValue = 10f;
    private Vector2 originalPosition;

    private void Awake()
    {
        originalPosition = textComponent.anchoredPosition;
    }

    public void UpText()
    {
        textComponent.anchoredPosition = originalPosition;
    }
    
    public void DownText()
    {
        textComponent.anchoredPosition = originalPosition + Vector2.down * moveValue;
    }
}
