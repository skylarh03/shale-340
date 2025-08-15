using UnityEngine;

public class WindMusicLayer : MonoBehaviour
{
    public static WindMusicLayer Instance;

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
        
        // Make sure object owning the GM isn't destroyed when
        // exiting a scene
        DontDestroyOnLoad(gameObject);
    }
}
