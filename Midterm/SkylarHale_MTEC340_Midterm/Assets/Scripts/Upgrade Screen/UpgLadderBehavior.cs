using UnityEngine;

public class UpgLadderBehavior : MonoBehaviour
{
    private Rigidbody2D _rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.linearVelocityY = -3.0f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Upgrade Screen Destroy Ladder")) Destroy(gameObject);
    }
}
