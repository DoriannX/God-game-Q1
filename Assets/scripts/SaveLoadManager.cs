using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private static SaveLoadManager instance;
    [SerializeField] private SaveLoadSystem.SaveLoadSystem saveLoadSystem;
    private bool loadSave;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            print("Instance already exists, destroying object!");
            Destroy(gameObject);
        }
        else
        {
            print("Instance created!");
            instance = this;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded: " + scene.name);
         Invoke(nameof(Init), 0.1f);
    }

    public void LoadSave()
    {
        loadSave = true;
    }

    public void Init()
    {
        if (loadSave)
        {
            SaveLoadSystem.SaveLoadSystem.Load();
        }
    }
    
    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}