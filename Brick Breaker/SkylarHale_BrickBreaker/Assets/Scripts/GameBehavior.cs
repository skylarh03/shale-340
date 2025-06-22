using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBehavior : MonoBehaviour
{
    public static GameBehavior Instance;
    
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

    private int bricksToDestroy = 33;
    private int bricksDestroyed = 0;

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
        ResetGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ScorePoints(int points)
    {
        Score += points;
        bricksDestroyed++;
        ScoreText.text = Score.ToString();
        
        // check if enough bricks have been destroyed
        if (bricksDestroyed >= bricksToDestroy)
        {
            Victory();
        }
    }

    public void LoseLife()
    {
        Lives -= 1;
        LivesText.text = Lives.ToString();
    }

    public void GameOver()
    {
        GameOverPanel.SetActive(true);
    }

    public void Victory()
    {
        StopBall();
        VictoryPanel.SetActive(true);
    }

    void StopBall()
    {
        ball.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
    }

    public void ResetGame()
    {
        GameOverPanel.SetActive(false);
        Destroy(brickLayoutInstance);
        brickLayoutInstance = Instantiate(brickLayoutPrefab); // resets brick layout to what it is in the default prefab
        
        Score = 0;
        Lives = 3;
        Level = 1;
        ScoreText.text = Score.ToString();
        LivesText.text = Lives.ToString();
        LevelText.text = Level.ToString();
        
        bricksDestroyed = 0;
    }

    public void NextLevel()
    {
        VictoryPanel.SetActive(false);
        Destroy(brickLayoutInstance);
        brickLayoutInstance = Instantiate(brickLayoutPrefab); // resets brick layout to what it is in the default prefab
        
        Level++;
        ScoreText.text = Score.ToString();
        LivesText.text = Lives.ToString();
        LevelText.text = Level.ToString();
        
        bricksDestroyed = 0;
    }
}
