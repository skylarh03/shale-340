using System.Collections;
using UnityEngine;

public class BulletBillBehavior : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 1.0f;
    [SerializeField] private float _lifetime = 10.0f;
    
    private Rigidbody2D _rb;

    private bool _isPaused;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        //Debug.Log(_rb);
        
        StartCoroutine(LifeCycle());
    }

    void Start()
    {
        _rb.linearVelocityX = _moveSpeed;
        
        GameBehavior.Instance._activeBullets.Add(gameObject);
    }

    void Update()
    {
        if (GameBehavior.Instance.CurrentState == Utilities.GameState.Pause)
        {
            if (!_isPaused)
            {
                _isPaused = true;
                
                _rb.linearVelocity = Vector2.zero;
            }
        }
        else if (GameBehavior.Instance.CurrentState == Utilities.GameState.Play)
        {
            if (_isPaused)
            {
                _isPaused = false;
                
                _rb.linearVelocityX = _moveSpeed;
            }
        }
        else if (GameBehavior.Instance.CurrentState == Utilities.GameState.WinLevel) _rb.linearVelocity = Vector2.zero;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameBehavior.Instance.LoseHealth();
            Destroy(gameObject);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // destroy bullet bill upon colliding with hammer
        if (other.gameObject.CompareTag("Hammer"))
        {
            GameBehavior.Instance.ScorePoints();
            Destroy(gameObject);
        }
    }

    public void SetInitialSpeed(Utilities.SpawnDirection spawnDirection)
    {
        switch (spawnDirection)
        {
            case Utilities.SpawnDirection.Left:
                _moveSpeed *= -1;
                break;
            case Utilities.SpawnDirection.Right:
                // speed stays the same
                break;
        }
    }

    public void IncreaseSpeed()
    {
        _moveSpeed += 0.4f;
    }

    public void ResetSpeed()
    {
        _moveSpeed = 2.0f;
    }
    
    IEnumerator LifeCycle()
    {
        yield return new WaitForSeconds(_lifetime);
        Destroy(gameObject);
    }
}
