using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameBehavior : MonoBehaviour
{
    public static GameBehavior Instance;
    
    [SerializeField] private int _currentLevel = 1;
    public Utilities.GameState CurrentState;

    [Header("Player Properties")]
    [SerializeField] private int _lives = 3;

    [SerializeField] private bool _playerIsAlive = true;
    [SerializeField] private PlayerScore _playerScore;

    [Header("Prefabs and Objects to Manipulate")] 
    [SerializeField] private GameObject _barrelPrefab;
    [SerializeField] private GameObject _fireEnemyPrefab;
    
    [SerializeField] private GameObject _barrelSpawnerPrefab;
    [SerializeField] private GameObject _fireSpawnerPrefab;

    private GameObject _barrelSpawnerInst;
    private GameObject _fireSpawnerInst;

    [Header("Lists of Active Objects")] 
    public List<GameObject> ActiveBarrels;
    public List<GameObject> ActiveFireEnemies;

    [Header("Powerup Information")] 
    public float PowerupDuration = 10.0f;

    [Header("Audio")] 
    public AudioSource Music;
    public AudioClip LevelMusic;
    public AudioClip PowerupMusic;
    
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

    void Start()
    {
        CurrentState = Utilities.GameState.Play;
        
        _barrelSpawnerInst = Instantiate(_barrelSpawnerPrefab);
        _barrelSpawnerInst.SetActive(true);
        
        _fireSpawnerInst = Instantiate(_fireSpawnerPrefab);
        _fireSpawnerInst.SetActive(true);
    }

    void Update()
    {
        if (CurrentState == Utilities.GameState.Death && _playerIsAlive)
        {
            _playerIsAlive = false;
            StartCoroutine(PlayerDeathEvents()); // waits for player death animation to play, then resets game
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
        _playerIsAlive = true;
        _currentLevel = 1;
        
        // reset all prefab and object parameters to default values
        _barrelPrefab.GetComponent<BarrelBehavior>().barrelSpeedX = 3.0f;
        
        _fireEnemyPrefab.GetComponent<FireEnemyBehavior>().defaultSpeed = 1.0f;
        _fireEnemyPrefab.GetComponent<FireEnemyBehavior>().chaseSpeed = 1.75f;

        _barrelSpawnerPrefab.GetComponent<BarrelSpawner>().minimumSpawnInterval = 3.0f;
        _barrelSpawnerPrefab.GetComponent<BarrelSpawner>().maximumSpawnInterval = 8.0f;

        _fireSpawnerPrefab.GetComponent<FireEnemySpawner>().minimumSpawnInterval = 25.0f;
        _fireSpawnerPrefab.GetComponent<FireEnemySpawner>().maximumSpawnInterval = 40.0f;
        
        // new instances of spawners
        _barrelSpawnerInst = Instantiate(_barrelSpawnerPrefab);
        _barrelSpawnerInst.SetActive(true);
        
        _fireSpawnerInst = Instantiate(_fireSpawnerPrefab);
        _fireSpawnerInst.SetActive(true);
        
        CurrentState = Utilities.GameState.Play;
    }

    IEnumerator PlayerDeathEvents()
    {
        Destroy(_barrelSpawnerInst);
        Destroy(_fireSpawnerInst);
        
        yield return new WaitForSeconds(4.75f);
        
        // destroy all active obstacles and spawner instances, empty lists, then reset the game
        ActiveBarrels.ForEach(x => Destroy(x));
        ActiveBarrels.RemoveAll(x=>x);

        ActiveFireEnemies.ForEach(x => Destroy(x));
        ActiveFireEnemies.RemoveAll(x=>x);
    }
}
