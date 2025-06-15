using System;
using UnityEngine;

public class BrickBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            Destroy(gameObject);
        }
    }

    void RandomizeSpriteColor()
    {
        // Randomize color of brick upon initialization
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Array of possible colors to choose from
        Color[] colors = new Color[7] {Color.blue, Color.cyan, Color.green, Color.magenta, Color.red, Color.white, Color.yellow};
        
        spriteRenderer.color = colors[UnityEngine.Random.Range(0, colors.Length)];
    }
}
