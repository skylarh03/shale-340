using UnityEngine;

// manager class - instance of the game
// software design pattern - singleton pattern
public class GameBehavior : MonoBehaviour
{
    public static GameBehavior Instance;

    public float PaddleSpeed = 5.0f;
    public float InitBallForce = 5.0f;
    public float PaddleInfluence = 0.4f;
    public float BallSpeedIncrement = 1.1f;

    [SerializeField] private int _pointsToVictory;

    [SerializeField] private Player[] _players = new Player[2];
    
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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ScorePoint(int playerNum)
    {
        _players[playerNum - 1].Score++;
        CheckWinner();
    }

    private void CheckWinner()
    {
        foreach (Player p in _players)
        {
            if (p.Score >= _pointsToVictory)
            {
                ResetGame();
            }
        }
    }

    private void ResetGame()
    {
        foreach (Player p in _players)
        {
            p.Score = 0;
        }
    }
}
