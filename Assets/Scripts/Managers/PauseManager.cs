using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button pauseButton;
    public bool isPaused { get; private set; }
    public event Action<bool> onPauseToggled;

    private void Awake()
    {
        resumeButton.onClick.AddListener(TogglePause);
        pauseButton.onClick.AddListener(TogglePause);
    }

    private void OnEnable()
    {
        inputHandler.pausePressed += TogglePause;
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        onPauseToggled?.Invoke(isPaused);
    }
}
