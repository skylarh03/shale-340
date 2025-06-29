using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBehavior : MonoBehaviour
{
    public static GameBehavior Instance;
    
    [SerializeField] private int _currentLevel = 1;
    public Utilities.GameState CurrentState;

    [Header("Player Properties")]
    [SerializeField] private PlayerBehavior _player;
    [SerializeField] private int _health = 3;
    [SerializeField] private int _maxHealth = 3;
    public bool HasWonLevel = false;

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
    public AudioSource SFX;
    public AudioClip LevelMusic;
    public AudioClip PowerupMusic;
    public AudioClip DeathMusic;
    public AudioClip WinLevelMusic;
    public AudioClip VictoryMusic;
    [SerializeField] private AudioClip _scoreSFX;
    [SerializeField] private AudioClip _playerHurtSFX;

    [Header("UI")]
    // score text is already handled, this is just for HP and level
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private TMP_Text _levelText;
    
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
        
        SFX = GetComponent<AudioSource>();
        
        // initialize UI attributes
        _healthText.text = $"{_health}/{_maxHealth} HP";
        _levelText.text = _currentLevel.ToString();
    }

    void Update()
    {
        if (CurrentState == Utilities.GameState.Death && _playerIsAlive)
        {
            _playerIsAlive = false;
            StartCoroutine(PlayerDeathEvents()); // waits for player death animation to play, then resets game
        }
        else if (CurrentState == Utilities.GameState.WinLevel && !HasWonLevel)
        {
            HasWonLevel = true;
            StartCoroutine(PlayerWinLevel());
        }
    }

    public void ScorePoints(int points = 100)
    {
        _playerScore.Score += points;
        
        // play score audio
        SFX.PlayOneShot(_scoreSFX);
    }

    public void LoseHealth()
    {
        _health--;
        _healthText.text = $"{_health}/{_maxHealth} HP";
        if (_health == 0)
        {
            CurrentState = Utilities.GameState.Death;
        }
        else
        {
            // since there's already a separate sound playing for death, play a damage sound otherwise
            Utilities.PlaySound(SFX, _playerHurtSFX);
        }
    }

    public void NextLevel()
    {
        _currentLevel++;
        _levelText.text = _currentLevel.ToString();
    }
    
    public void ResetGame()
    {
        _health = 3;
        _healthText.text = $"{_health}/{_maxHealth} HP";
        _playerScore.Score = 0;
        _playerIsAlive = true;
        _currentLevel = 1;
        _levelText.text = _currentLevel.ToString();
        
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

        _player.ResetPlayer();
        
        Utilities.PlaySound(Music, LevelMusic, loop: true);
        
        CurrentState = Utilities.GameState.Play;
    }

    IEnumerator PlayerDeathEvents()
    {
        Destroy(_barrelSpawnerInst);
        Destroy(_fireSpawnerInst);
        
        Utilities.PlaySound(Music, DeathMusic);
        
        yield return new WaitForSeconds(3.65f);
        
        DestroyAllActiveObjects();
        CurrentState = Utilities.GameState.GameOver;
        SceneManager.LoadScene("Scenes/GameOver", LoadSceneMode.Additive);
    }

    IEnumerator PlayerWinLevel()
    {
        Utilities.PlaySound(Music, WinLevelMusic);
        yield return new WaitForSeconds(2.0f);
        
        DestroyAllActiveObjects();
        Utilities.PlaySound(Music, VictoryMusic, loop: true);
        CurrentState =  Utilities.GameState.Play;
    }

    void DestroyAllActiveObjects()
    {
        // destroy all active obstacles and spawner instances, empty lists, then reset the game
        ActiveBarrels.ForEach(x => Destroy(x));
        ActiveBarrels.RemoveAll(x=>x);
        Destroy(_barrelSpawnerInst);

        ActiveFireEnemies.ForEach(x => Destroy(x));
        ActiveFireEnemies.RemoveAll(x=>x);
        Destroy(_fireSpawnerInst);
    }
}
