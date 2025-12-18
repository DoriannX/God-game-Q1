using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private float offset;
    [SerializeField] private PauseManager pauseManager;
    [SerializeField] private SaveUI saveUI;
    

    public enum Buttons
    {
        Resume,
        Save,
        Load,
        Quit
    }

    [SerializeField] private List<Button> buttons;

    [SerializeField] private Button saveQuitButton;

    private List<float> initialWidths = new List<float>();
    private const string mainMenuName = "MainMenu";

    private void Awake()
    {
        foreach (var button in buttons)
        {
            var rectTransform = button.GetComponent<RectTransform>();
            initialWidths.Add(rectTransform.sizeDelta.x);
        }

        pauseManager.onPauseToggled += Toggle;
        //saveButton.onClick.AddListener(() =>
        //{
          //  SaveLoadSystem.SaveLoadSystem.SaveNew();
            //saveUI.Display();
        //});
        saveQuitButton.onClick.AddListener(() =>
        {
            SaveLoadSystem.SaveLoadSystem.SaveNew(() => { StartCoroutine(QuitWithUnscaledDelay(0.1f)); });
        });
        Toggle(pauseManager.isPaused);
    }

    private void OnEnable()
    {
        Select(-1);
    }

    private System.Collections.IEnumerator QuitWithUnscaledDelay(float delay)
    {
       yield return new WaitForSecondsRealtime(delay);

        Quit();
    }

    private void Quit()
    {
        SceneManager.LoadScene(mainMenuName);
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
                ).SetEase(Ease.OutBack).SetUpdate(true);
            }
            else
            {
                rectTransform.DOSizeDelta(
                    new Vector2(initialWidths[i], rectTransform.sizeDelta.y),
                    0.2f
                ).SetEase(Ease.OutBack).SetUpdate(true);
                ;
            }
        }
    }

    private void Toggle(bool isOn)
    {
        gameObject.SetActive(isOn);
    }
}