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
        if (GameBehavior.Instance.CurrentState == Utilities.GameState.Play || GameBehavior.Instance.CurrentState == Utilities.GameState.Pause)
        {
            _sr.enabled = true;
            _col.enabled = true;
        }
        else
        {
            _sr.enabled = false;
            _col.enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) Destroy(gameObject);
    }
}
