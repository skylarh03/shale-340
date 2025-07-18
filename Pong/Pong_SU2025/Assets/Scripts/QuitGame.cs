using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class QuitGame : MonoBehaviour
{
    public static QuitGame Instance;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else 
            Instance = this;
        
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Quit();
    }

    void Quit()
    {
        #if UNITY_EDITOR
            // function runs when game is running from unity
            EditorApplication.isPlaying = false;
        #else
            // function runs when game is running from build
            Application.Quit();
        #endif
    }
}
