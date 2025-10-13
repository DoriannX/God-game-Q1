using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private float offset;

    public enum Buttons
    {
        Resume,
        Save,
        Load,
        Quit
    }

    [SerializeField] private List<Button> buttons;

    private List<float> initialWidths = new List<float>();

    private void Awake()
    {
        foreach (var button in buttons)
        {
            var rectTransform = button.GetComponent<RectTransform>();
            initialWidths.Add(rectTransform.sizeDelta.x);
        }
    }

    public void Select(int button)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            RectTransform rectTransform = buttons[i].GetComponent<RectTransform>();
            if (i == button)
            {
                rectTransform.DOSizeDelta(
                    new Vector2(initialWidths[i] + offset, rectTransform.sizeDelta.y),
                    0.2f
                ).SetEase(Ease.OutBack);
            }
            else
            {
                rectTransform.DOSizeDelta(
                    new Vector2(initialWidths[i], rectTransform.sizeDelta.y),
                    0.2f
                ).SetEase(Ease.OutBack);
            }
        }
    }
}