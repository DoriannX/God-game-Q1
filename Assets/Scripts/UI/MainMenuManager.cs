using System;
using UnityEngine;
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

    private void Awake()
    {
        newGameButton.onClick.AddListener(LoadGame);

        OptionsButton.onClick.AddListener(Options);

        creditsButton.onClick.AddListener(Credits);

        continueButton.onClick.AddListener(() =>
        {
            saveLoadManager.LoadSave();
            LoadGame();
        });

        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        });
    }

    private void LoadGame()
    {
        SceneManager.LoadScene("Scenes/SampleScene");
    }

    private void Options()
    {
        SceneManager.LoadScene("Scenes/Options");
    }

    private void Credits()
    {
               SceneManager.LoadScene("Scenes/Credits");
    }
}