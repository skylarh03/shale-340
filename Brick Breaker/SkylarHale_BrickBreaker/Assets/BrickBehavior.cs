using System;
using UnityEngine;

public class BrickBehavior : MonoBehaviour
{
    private Color[] _colors = new Color[7]
        {Color.blue, Color.cyan, Color.green, Color.magenta, Color.red, Color.white, Color.yellow};

    [SerializeField] private Color activeColor = Color.white;

    public int value = 100;
    
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
            // add brick's value to player's score
            GameBehavior.Instance.ScorePoints(value);
            
            // get rid of brick
            Destroy(gameObject);
        }
    }

    // randomize color of brick upon initialization
    // depending on the color, update its score value
    void RandomizeSpriteColor()
    {
        int colorIndex = UnityEngine.Random.Range(0, _colors.Length);
        
        _sr.color = _colors[colorIndex];
        activeColor = _sr.color; // for inspector viewing on the script component
        
        // depending on the color, assign new score value
        switch (colorIndex)
        {
            case 0: // blue
                value = 300;
                break;
            case 1: // cyan
                value = 400;
                break;
            case 2: // green
                value = 200;
                break;
            case 3: // magenta
                value = 600;
                break;
            case 4: // red
                value = 500;
                break;
            case 5: // white
                value = 100;
                break;
            case 6:
                value = 700;
                break;
        }
    }
}
