using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
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
    [SerializeField] private List<GameObject> _defaultPrefabs;
    [SerializeField] private GameObject _barrelPrefab;
    [SerializeField] private GameObject _fireEnemyPrefab;
    [SerializeField] private GameObject _barrelSpawnerPrefab;
    [SerializeField] private GameObject _fireSpawnerPrefab;

    private BarrelBehavior _currentBarrelInfo;
    private FireEnemyBehavior _currentFireInfo;
    private BarrelSpawner _currentBarrelSpawnerInfo;
    private FireEnemySpawner  _currentFireSpawnerInfo;

    [Header("Active Spawners")]
    [SerializeField] private List<GameObject> __activeBarrelspawners =  new List<GameObject>();
    [SerializeField] private List<GameObject> _activeFireSpawners =  new List<GameObject>();
    
    [HideInInspector] public List<GameObject> _activeBarrels;
    [HideInInspector] public List<GameObject> _activeFireEnemies;
    [Header("Level Environments")] 
    public List<LevelEnvironment> LevelEnvironments;
    [SerializeField] private LevelEnvironment _currentLevelEnv;

    [Header("Pickup Prefabs")]
    public GameObject BonusPointPickup;
    public GameObject BonusHealthPickup;
    
    [Header("Powerup Information")] 
    public List<Utilities.Powerups> UnlockedPowerups = new List<Utilities.Powerups>()
    {
        Utilities.Powerups.SuperHammer
    };
    public GameObject HammerPrefab;
    public float HammerDuration = 10.0f;
    public float BoomerangDuration = 10.0f;
    public float IceDuration = 10.0f;

    [Header("Audio")] 
    public AudioSource Music;
    public AudioSource SFX;
    public AudioClip TitleMusic;
    public AudioClip LevelMusic;
    public AudioClip PowerupMusic;
    public AudioClip DeathMusic;
    public AudioClip WinLevelMusic;
    public AudioClip VictoryMusic;
    public AudioClip UpgradeMusic;
    [SerializeField] private AudioClip _scoreSFX;
    [SerializeField] private AudioClip _playerHurtSFX;
    [SerializeField] private AudioClip _pauseSFX;
    [SerializeField] private AudioClip _selectSFX;
    
    [Header("Cutscene Objects")]
    [SerializeField] private SpriteRenderer _whiteTransitionScreen;

    [Header("UI")]
    // score text is already handled, this is just for HP and level
    [SerializeField] private GameObject _levelUICanvas;
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
        Utilities.PlaySound(Music, TitleMusic, loop: true);
        
        CurrentState = Utilities.GameState.TitleScreen;
        SceneManager.LoadScene("TitleScreen", LoadSceneMode.Additive);
        _levelUICanvas.SetActive(false);
        
        SFX = GetComponent<AudioSource>();
        
        _currentBarrelInfo = _barrelPrefab.GetComponent<BarrelBehavior>();
        _currentFireInfo = _fireEnemyPrefab.GetComponent<FireEnemyBehavior>();
        
        _currentBarrelSpawnerInfo = _barrelSpawnerPrefab.GetComponent<BarrelSpawner>();
        _currentFireSpawnerInfo = _fireSpawnerPrefab.GetComponent<FireEnemySpawner>();
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
        
        // manage pausing
        if (Input.GetKeyDown(KeyCode.Escape) && CurrentState is Utilities.GameState.Play or Utilities.GameState.Pause)
        {
            // manage pause menu
            if (CurrentState == Utilities.GameState.Play)
            {
                SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
                Utilities.PlaySound(SFX, _pauseSFX);
                Music.volume = 0.5f;
            }
            else
            {
                SceneManager.UnloadSceneAsync("PauseMenu");
                Utilities.PlaySound(SFX, _selectSFX);
                Music.volume = 1.0f;
            }
            
            CurrentState = CurrentState == Utilities.GameState.Play 
                ? Utilities.GameState.Pause 
                : Utilities.GameState.Play;
        }
    }

    public void ScorePoints(int points = 100)
    {
        _playerScore.Score += points;
        
        // play score audio
        Utilities.PlaySound(SFX, _scoreSFX);
    }

    // increase health by 1 if not max hp
    // otherwise, add to score by 100
    public void HealthPickup()
    {
        if (_health < _maxHealth)
        {
            _health++;
            _healthText.text = $"{_health}/{_maxHealth} HP";
        }
        else _playerScore.Score += 100;
        
        // some sound effect here
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

        HasWonLevel = false;
        
        _currentLevelEnv = Instantiate(LevelEnvironments[(_currentLevel - 1) % LevelEnvironments.Count]);
        
        // increase obstacle speed + spawner frequencies
        _currentBarrelInfo.IncreaseSpeed();
        _currentFireInfo.IncreaseSpeed();
        _currentBarrelSpawnerInfo.IncreaseSpawnFrequency();
        _currentFireSpawnerInfo.IncreaseSpawnFrequency();
        
        // assign spawners to corresponding locations in the level prefab
        // based off of the location gameobjects
        
        // barrel spawner(s)
        for (int i = 0; i < _currentLevelEnv.BarrelSpawnerLocations.Count; i++)
        {
            GameObject newSpawner = Instantiate(_barrelSpawnerPrefab, _currentLevelEnv.BarrelSpawnerLocations[i].transform);
            newSpawner.SetActive(true);
            __activeBarrelspawners.Add(newSpawner);
        }
        
        // fire enemy spawner(s)
        for (int i = 0; i < _currentLevelEnv.FireEnemySpawnerLocations.Count; i++)
        {
            GameObject newSpawner = Instantiate(_fireSpawnerPrefab, _currentLevelEnv.FireEnemySpawnerLocations[i].transform);
            newSpawner.SetActive(true);
            _activeFireSpawners.Add(newSpawner);
        }
        
        _player.ResetPlayer(_currentLevelEnv.PlayerSpawnLocation.transform.position);

        CurrentState = Utilities.GameState.Play;
    }
    
    public void ResetGame()
    {
        // reset all values to defaults
        _health = 3;
        _maxHealth = 3;
        _healthText.text = $"{_health}/{_maxHealth} HP";
        
        _playerScore.Score = 0;
        _player.horizontalSpeed = 1.5f;
        _player.climbSpeed = 1.5f;
        _player.jumpForce = 5.25f;
        _playerIsAlive = true;
        HasWonLevel = false;
        
        _currentLevel = 1;
        _levelText.text = _currentLevel.ToString();
        
        // reset all prefab and object parameters to default values
        // default instances of all prefabs are stored in GameBehavior as well
        _currentBarrelInfo.ResetSpeed();
        _currentFireInfo.ResetSpeed();
        _currentBarrelSpawnerInfo.ResetSpawnFrequency();
        _currentFireSpawnerInfo.ResetSpawnFrequency();
        
        // reset unlocked powerups
        UnlockedPowerups.Clear();
        UnlockedPowerups.Add(Utilities.Powerups.SuperHammer);
        
        // new instance of level 1 layout
        _currentLevelEnv = Instantiate(LevelEnvironments[0]);
        
        // assign spawners to corresponding locations in the level prefab
        // based off of the location gameobjects
        
        // barrel spawner(s)
        for (int i = 0; i < _currentLevelEnv.BarrelSpawnerLocations.Count; i++)
        {
            GameObject newSpawner = Instantiate(_barrelSpawnerPrefab, _currentLevelEnv.BarrelSpawnerLocations[i].transform);
            newSpawner.SetActive(true);
            __activeBarrelspawners.Add(newSpawner);
        }
        
        // fire enemy spawner(s)
        for (int i = 0; i < _currentLevelEnv.FireEnemySpawnerLocations.Count; i++)
        {
            GameObject newSpawner = Instantiate(_fireSpawnerPrefab, _currentLevelEnv.FireEnemySpawnerLocations[i].transform);
            newSpawner.SetActive(true);
            _activeFireSpawners.Add(newSpawner);
        }

        _player.ResetPlayer(_currentLevelEnv.PlayerSpawnLocation.transform.position);
        
        Utilities.PlaySound(Music, LevelMusic, loop: true);
        
        CurrentState = Utilities.GameState.Play;
        
        _levelUICanvas.SetActive(true);
        
        SceneManager.UnloadSceneAsync("TitleScreen");
    }

    IEnumerator PlayerDeathEvents()
    {
        DestroyAllSpawners();
        
        Utilities.PlaySound(Music, DeathMusic);
        
        yield return new WaitForSeconds(3.65f);
        
        DestroyAllActiveObjects();
        CurrentState = Utilities.GameState.GameOver;
        SceneManager.LoadScene("Scenes/GameOver", LoadSceneMode.Additive);
        Destroy(_currentLevelEnv.gameObject);
        
        _levelUICanvas.SetActive(false);
    }

    IEnumerator PlayerWinLevel()
    {
        Utilities.PlaySound(Music, WinLevelMusic);
        yield return new WaitForSeconds(2.0f);
        
        DestroyAllActiveObjects();
        Utilities.PlaySound(Music, VictoryMusic, loop: true);
        CurrentState =  Utilities.GameState.Play;
    }

    IEnumerator FadeToWhite()
    {
        float timer = 0.0f;

        // fade screen to white over a period of time
        // also fade out music, and play swell sfx to go with it (need to design this)
        while (timer < 2.5f)
        {
            timer += 0.01f;
            _whiteTransitionScreen.color += new Color(0, 0, 0, 0.004f);
            Music.volume -= 0.004f;
            yield return new WaitForSeconds(0.01f);
        }
        
        Music.Stop();
        
        yield return new WaitForSeconds(1.5f);
        Destroy(_currentLevelEnv.gameObject);
        Utilities.PlaySound(Music, UpgradeMusic, loop: true);
        Music.volume = 1.0f;

        SceneManager.LoadScene("UpgradeScreen", LoadSceneMode.Additive);
        
        timer = 0.0f;
        // fade out white to show upgrade screen
        while (timer < 1.0f)
        {
            timer += 0.01f;
            _whiteTransitionScreen.color -= new Color(0, 0, 0, 0.01f);
            yield return new WaitForSeconds(0.01f);
        }
    }

    void DestroyAllActiveObjects()
    {
        // destroy all active obstacles and spawner instances, empty lists, then reset the game
        _activeBarrels.ForEach(Destroy);
        _activeBarrels.RemoveAll(x=>x);

        _activeFireEnemies.ForEach(Destroy);
        _activeFireEnemies.RemoveAll(x=>x);
    }

    void DestroyAllSpawners()
    {
        __activeBarrelspawners.ForEach(Destroy);
        __activeBarrelspawners.RemoveAll(x=>x);
        _activeFireSpawners.ForEach(Destroy);
        _activeFireSpawners.RemoveAll(x=>x);
    }

    public void TransitionToPointsShop()
    {
        StartCoroutine(FadeToWhite());
    }
    
    // methods for applying upgrades are below

    // increase movement speed by 20% of initial value (1.5)
    public void ApplyDashPepper()
    {
        _player.horizontalSpeed += 0.3f;
        _player.climbSpeed += 0.3f;
    }

    public void ApplySuperMushroom()
    {
        _maxHealth += 1;
        _health += 1;
        _healthText.text = $"{_health}/{_maxHealth} HP";
    }
    
    // increase jump force by 0.75
    public void ApplyGoombaShoe()
    {
        _player.jumpForce += 0.75f;
    }
    
    // increase duration of hammer powerup
    public void ApplySuperHammer()
    {
        HammerDuration += 2.0f;
    }
    
    // has two possible actions:
    // if boomerang flower is not unlocked, unlock it
    // if it is unlocked, increase the powerup duration
    public void ApplyBoomerangFlower()
    {
        if (!UnlockedPowerups.Contains(Utilities.Powerups.BoomerangFlower))
        {
            UnlockedPowerups.Add(Utilities.Powerups.BoomerangFlower);
            Utilities.UpgradeDescriptions[6] = "Increases duration of Boomerang Flower";
        }
        else BoomerangDuration += 2.0f;
    }
    
    // same as above but applied to ice flower
    public void ApplyIceFlower()
    {
        if (!UnlockedPowerups.Contains(Utilities.Powerups.IceFlower))
        {
            UnlockedPowerups.Add(Utilities.Powerups.IceFlower);
            Utilities.UpgradeDescriptions[7] = "Increases duration of Ice Flower";
        }
        else IceDuration += 2.0f;
    }
}
