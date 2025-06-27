using UnityEngine;
using System.Collections;

public class FireEnemyBehavior : MonoBehaviour
{
    [SerializeField] private float defaultSpeed = 1.0f;
    [SerializeField] private float chaseSpeed = 1.75f;
    private float currentSpeed;

    private float _direction = 0.0f;
    private float _verticalDirection = 0.0f;
    
    [SerializeField] private bool doesSeePlayer = false;
    [SerializeField] private bool isMoving = false;
    [SerializeField] private bool isClimbing = false;
    
    private Rigidbody2D _rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        StartCoroutine(IdleMove());
    }

    void FixedUpdate()
    {
        _rb.linearVelocityX = currentSpeed * _direction;
    }

    IEnumerator IdleMove()
    {
        // determine rotation of enemy before moving
        if (Random.Range(0.0f, 1.0f) < 0.5)
        {
            _rb.rotation = 180;
            _direction = -1.0f;
        }
        else
        {
            _rb.rotation = 0;
            _direction = 1.0f;
        }

        // randomly determine amount of time spent moving, then move that amount of time
        float timeSpentMoving = Random.Range(0.5f, 6.0f);
        currentSpeed = defaultSpeed;
        yield return new WaitForSeconds(timeSpentMoving);
        
        // after waiting that amount of time, stop moving
        currentSpeed = 0;
        _direction = 0.0f;
        isMoving = false;
        StartCoroutine(IdleWait());
    }

    IEnumerator IdleWait()
    {
        yield return new WaitForSeconds(Random.Range(0.0f, 0.3f));
        isMoving = true;
        StartCoroutine(IdleMove());
    }
}
