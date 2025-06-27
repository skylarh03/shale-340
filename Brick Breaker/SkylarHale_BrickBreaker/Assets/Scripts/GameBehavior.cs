using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBehavior : MonoBehaviour
{
    public static GameBehavior Instance;

    public enum GameState
    {
        Play,
        Pause,
        GameOver
    };

    public GameState CurrentState;

    [SerializeField] private TMP_Text _messagesUI;
    
    [Header("Game Parameters")]
    public float PaddleSpeed = 5.0f;
    public float InitBallForce = 5.0f;
    public float PaddleInfluence = 0.4f;
    public float BallSpeedIncrement = 1.1f;
    
    [Header("Player Information")]
    public int Score = 0;
    public int Lives = 3;
    public int Level = 1;
    
    [Header("UI Elements")]
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI LivesText;
    public TextMeshProUGUI LevelText;
    public GameObject GameOverPanel;
    public GameObject VictoryPanel;

    [Header("Bricks and Ball")]
    [SerializeField] private GameObject brickLayoutPrefab;

    [SerializeField] private GameObject brickLayoutInstance;
    [SerializeField] private GameObject ball;

    public int bricksToDestroy = 33;
    public int bricksDestroyed = 0;

    [Header("Audio")] 
    [SerializeField] private AudioSource _musicSource;
    private AudioSource _uiSfxSource;
    [SerializeField] private AudioClip _lifeUpClip;

    private bool givenBonusLife = false;

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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentState = GameState.Play;
        _messagesUI.enabled = false;
        
        _uiSfxSource = GetComponent<AudioSource>();
        
        ResetGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (CurrentState == GameState.Play)
            {
                CurrentState = GameState.Pause;
                _messagesUI.enabled = true;
            }
            else if (CurrentState == GameState.Pause)
            {
                CurrentState = GameState.Play;
                _messagesUI.enabled = false;
            }
            
            PlayUISound();
        }

        if (Lives <= 0 || bricksDestroyed >= bricksToDestroy)
        {
            CurrentState = GameState.GameOver;
        }
        
        // life bonus every 3000 points
        if (Score % 3000 == 0 && Score != 0 && !givenBonusLife)
        {
            Lives++;
            LivesText.text = Lives.ToString();
            _uiSfxSource.PlayOneShot(_lifeUpClip);
            givenBonusLife = true;
        }
        else if (Score % 3000 != 0 && givenBonusLife)
        {
            givenBonusLife = false;
        }
    }

    public void ScorePoints(int points)
    {
        Score += points;
        ScoreText.text = Score.ToString();
    }

    public void LoseLife()
    {
        Lives -= 1;
        LivesText.text = Lives.ToString();
    }

    public void GameOver()
    {
        GameOverPanel.SetActive(true);
        _musicSource.pitch = 0.85f;
    }

    public void Victory()
    {
        StopBall();
        VictoryPanel.SetActive(true);
    }

    public void StopBall()
    {
        ball.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
    }

    public void ResetGame()
    {
        GameOverPanel.SetActive(false);
        CurrentState = GameState.Play;
        Destroy(brickLayoutInstance);
        brickLayoutInstance = Instantiate(brickLayoutPrefab); // resets brick layout to what it is in the default prefab
        
        Score = 0;
        Lives = 3;
        Level = 1;
        ScoreText.text = Score.ToString();
        LivesText.text = Lives.ToString();
        LevelText.text = Level.ToString();
        
        bricksDestroyed = 0;

        _musicSource.pitch = 1f;
    }

    public void NextLevel()
    {
        VictoryPanel.SetActive(false);
        CurrentState = GameState.Play;
        Destroy(brickLayoutInstance);
        brickLayoutInstance = Instantiate(brickLayoutPrefab); // resets brick layout to what it is in the default prefab
        
        Level++;
        ScoreText.text = Score.ToString();
        LivesText.text = Lives.ToString();
        LevelText.text = Level.ToString();
        
        bricksDestroyed = 0;
    }

    public void PlayUISound()
    {
        _uiSfxSource.Play();
    }
}
