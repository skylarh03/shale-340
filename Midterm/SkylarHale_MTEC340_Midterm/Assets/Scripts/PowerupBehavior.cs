using UnityEngine;

public class PowerupBehavior : MonoBehaviour
{
    private SpriteRenderer _sr;
    private BoxCollider2D _col;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _col = GetComponent<BoxCollider2D>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (GameBehavior.Instance.CurrentState != Utilities.GameState.Play)
        {
            _sr.enabled = false;
            _col.enabled = false;
        }
        else
        {
            _sr.enabled = true;
            _col.enabled = true;
        }
    }
}
