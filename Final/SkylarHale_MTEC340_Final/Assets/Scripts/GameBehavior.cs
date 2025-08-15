using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBehavior : MonoBehaviour
{
    public static GameBehavior Instance;
    
    public Utilities.PauseState PauseState;
    
    [SerializeField] private KeyCode _pauseKey;
    private bool _isPaused = false;

    void Awake()
    {
        // Singleton pattern initializer
        if (Instance != null && Instance != this)
            Destroy(this);
        else
        {
            // Assign Game Manager (GM) if none exists
            Instance = this;
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PauseState = Utilities.PauseState.Off;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(_pauseKey) && !_isPaused)
        {
            _isPaused = true;
            PauseState = Utilities.PauseState.On;
            SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
            Time.timeScale = 0;
        }
        else if (Input.GetKeyDown(_pauseKey) && _isPaused)
        {
            _isPaused = false;
            PauseState = Utilities.PauseState.Off;
            SceneManager.UnloadSceneAsync("PauseMenu");
            Time.timeScale = 1;
        }
    }
}
