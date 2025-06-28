using Unity.VisualScripting;
using UnityEngine;

public class GameBehavior : MonoBehaviour
{
    public static GameBehavior Instance;
    
    [SerializeField] private int _currentLevel = 1;

    [Header("Player Properties")]
    [SerializeField] private int _lives = 3;
    [SerializeField] private PlayerScore _playerScore;

    [Header("Prefabs and Objects to Manipulate")] 
    [SerializeField] private GameObject _barrelPrefab;
    [SerializeField] private GameObject _fireEnemyPrefab;
    [SerializeField] private GameObject _barrelSpawner;
    [SerializeField] private GameObject _fireSpawner;
    
    // initializer: runs before start()
    void Awake()
    {
        // singleton pattern initializer
        if (Instance != null && Instance != this)
            Destroy(this);
        else
        {
            // assign Game Manager (GM) if none exists
            Instance = this;
            
            // make sure object owning the GM isn't destroyed when
            // exiting the scene
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ScorePoints(int points = 100)
    {
        _playerScore.Score += points;
    }

    public void LoseLife()
    {
        _lives--;
        if (_lives == 0)
        {
            ResetGame();
        }
    }

    public void NextLevel()
    {
        _currentLevel++;
    }
    
    void ResetGame()
    {
        _lives = 3;
        _playerScore.Score = 0;
        _currentLevel = 1;
        
        // reset all prefab and object parameters to default values
        _barrelPrefab.GetComponent<BarrelBehavior>().barrelSpeedX = 3.0f;
        
        _fireEnemyPrefab.GetComponent<FireEnemyBehavior>().defaultSpeed = 1.0f;
        _fireEnemyPrefab.GetComponent<FireEnemyBehavior>().chaseSpeed = 1.75f;

        _barrelSpawner.GetComponent<BarrelSpawner>().minimumSpawnInterval = 3.0f;
        _barrelSpawner.GetComponent<BarrelSpawner>().maximumSpawnInterval = 8.0f;

        _fireSpawner.GetComponent<FireEnemySpawner>().minimumSpawnInterval = 25.0f;
        _fireSpawner.GetComponent<FireEnemySpawner>().maximumSpawnInterval = 40.0f;
    }
}
