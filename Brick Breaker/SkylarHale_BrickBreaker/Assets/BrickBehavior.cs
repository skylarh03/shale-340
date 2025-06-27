using UnityEngine;

public class BrickBehavior : MonoBehaviour
{
    private readonly Color[] _colors = new Color[7]
        {Color.white, Color. green, Color.blue, Color.cyan, Color.red, Color.magenta, Color.yellow};

    [SerializeField] private Color activeColor = Color.white;

    public int ScoreValue = 100;

    private int brickLives = 1;
    
    private SpriteRenderer _sr;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        RandomizeSpriteColor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            // add brick's ScoreValue to player's score
            GameBehavior.Instance.ScorePoints(ScoreValue);
            
            // lower brickLives by 1. if it hits 0, destroy the brick
            brickLives--;
            if (brickLives == 0)
            {
                Destroy(gameObject);
                GameBehavior.Instance.bricksDestroyed++;
            }
            else _sr.color = _colors[brickLives - 1];

            if (GameBehavior.Instance.bricksDestroyed == GameBehavior.Instance.bricksToDestroy)
            {
                GameBehavior.Instance.StopBall();
                GameBehavior.Instance.Victory();
            }
        }
    }

    // randomize color of brick upon initialization
    // depending on the color, update its score ScoreValue
    void RandomizeSpriteColor()
    {
        int colorIndex;
        float randomPercent = Random.Range(0.0f, 1.0f);
        
        // assign color index based on percentage values
        if (randomPercent < 0.35f) colorIndex = 0;
        else if (randomPercent < 0.60f) colorIndex = 1;
        else if (randomPercent < 0.80f) colorIndex = 2;
        else if (randomPercent < 0.90f) colorIndex = 3;
        else if (randomPercent < 0.95f) colorIndex = 4;
        else if (randomPercent < 0.99f) colorIndex = 5;
        else colorIndex = 6;
        
        _sr.color = _colors[colorIndex];
        activeColor = _sr.color; // for inspector viewing on the script component
        
        // depending on the color, assign new amount of lives
        brickLives = colorIndex + 1;
    }
}
