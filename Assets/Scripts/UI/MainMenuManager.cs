using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private SaveLoadManager saveLoadManager;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button OptionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    // Noms de sc�nes � utilisez exactement les noms pr�sents dans vos Build Settings.
    private const string SampleSceneName = "Doriann2";
    private const string OptionsSceneName = "Options";
    private const string CreditsSceneName = "Credits";
    private const string MainMenuSceneName = "mainMenu";

    private void Awake()
    {
        // Ajoute les listeners de mani�re s�re (�vite les NRE si un champ n'est pas assign�)
        SafeAddListener(newGameButton, LoadGame, nameof(newGameButton));
        SafeAddListener(OptionsButton, Options, nameof(OptionsButton));
        SafeAddListener(creditsButton, Credits, nameof(creditsButton));

        if (continueButton != null)
        {
            if (saveLoadManager != null)
            {
                continueButton.onClick.AddListener(() =>
                {
                    saveLoadManager.LoadSave();
                    LoadGame();
                });
            }
            else
            {
                continueButton.interactable = false;
                Debug.LogWarning($"[{nameof(MainMenuManager)}] {nameof(saveLoadManager)} non assign� : {nameof(continueButton)} d�sactiv�.");
            }
        }
        else
        {
            Debug.LogWarning($"[{nameof(MainMenuManager)}] {nameof(continueButton)} non assign�.");
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(() =>
            {
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            });
        }
        else
        {
            Debug.LogWarning($"[{nameof(MainMenuManager)}] {nameof(quitButton)} non assign�.");
        }
    }

    private void SafeAddListener(Button btn, UnityAction action, string fieldName)
    {
        if (btn == null)
        {
            Debug.LogWarning($"[{nameof(MainMenuManager)}] {fieldName} non assign�.");
            return;
        }

        btn.onClick.AddListener(action);
    }

    private void LoadGame()
    {
        SceneManager.LoadScene(SampleSceneName);
    }

    private void Options()
    {
        SceneManager.LoadScene(OptionsSceneName);
    }

    private void Credits()
    {
        SceneManager.LoadScene(CreditsSceneName);
    }

    public void Back()
    {
        SceneManager.LoadScene(MainMenuSceneName);
    }
}