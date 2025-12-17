
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class Button_UI : MonoBehaviour
{
    [SerializeField] private GameObject targetImage;
    [SerializeField] private bool startVisible = false;

    private Button button;
    private UnityAction toggleAction;

    private GameObject Target => targetImage != null ? targetImage : gameObject;

    private static readonly List<Button_UI> allButtons = new List<Button_UI>();

    private void Awake()
    {
        Target.SetActive(startVisible);
        button = GetComponent<Button>();
        toggleAction = Toggle;
        if (button != null)
        {
            button.onClick.AddListener(toggleAction);
        }
    }

    private void OnEnable()
    {
        if (!allButtons.Contains(this)) allButtons.Add(this);
        if (Target.activeSelf)
        {
            for (int i = 0; i < allButtons.Count; i++)
            {
                var b = allButtons[i];
                if (b != this)
                    b.SetVisible(false);
            }
        }
    }

    private void OnDisable()
    {
        allButtons.Remove(this);
    }

    private void OnDestroy()
    {
        if (button != null && toggleAction != null)
        {
            button.onClick.RemoveListener(toggleAction);
        }
        allButtons.Remove(this);
    }

    public void Toggle()
    {
        bool willBeVisible = !Target.activeSelf;
        if (willBeVisible)
        {
            for (int i = 0; i < allButtons.Count; i++)
            {
                var b = allButtons[i];
                if (b != this)
                    b.SetVisible(false);
            }
        }
        SetVisible(willBeVisible);
    }

    public void SetVisible(bool visible)
    {
        Target.SetActive(visible);
    }

    public bool IsVisible => Target.activeSelf;
}