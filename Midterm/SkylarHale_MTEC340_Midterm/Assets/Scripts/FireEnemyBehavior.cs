using UnityEngine;
using System.Collections;

public class FireEnemyBehavior : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float defaultSpeed = 1.0f;
    [SerializeField] private float chaseSpeed = 1.75f;
    private float currentSpeed;

    private float _direction = 0.0f;
    private float _verticalDirection = 0.0f;
    
    [Header("Conditionals")]
    [SerializeField] private bool doesSeePlayer = false;
    [SerializeField] private bool isClimbing = false;
    
    private Rigidbody2D _rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.linearDamping = 0;
        _rb.angularDamping = 0;
        _rb.gravityScale = 1;
        StartCoroutine(IdleMove());
    }

    void FixedUpdate()
    {
        _rb.linearVelocityX = currentSpeed * _direction;
        if (isClimbing) _rb.linearVelocityY = defaultSpeed * _verticalDirection;
    }

    // this only runs if the enemy is climbing down a ladder and reaches the bottom
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor") && isClimbing && _verticalDirection < 0.0f)
        {
            DisableClimbing();
            StopAllCoroutines();
            StartCoroutine(IdleMove());
        }
    }
    
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ladder"))
        {
            // if enemy is not currently climbing and is colliding with a ladder, set a 1% chance for every frame colliding to override all current movement and start climbing the ladder
            if (!isClimbing && Random.Range(0.0f, 1.0f) < 0.01)
            {
                EnableClimbing();
                StopAllCoroutines();
                StartCoroutine(ClimbUp());
            }
        }

        if (other.gameObject.CompareTag("Ladder Top"))
        {
            if (!isClimbing && Random.Range(0.0f, 1.0f) < 0.01)
            {
                EnableClimbing();
                StopAllCoroutines();
                StartCoroutine(ClimbDown());
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ladder") && isClimbing)
        {
            DisableClimbing();
            StopAllCoroutines();
            StartCoroutine(IdleMove());
        }
    }

    void EnableClimbing()
    {
        Debug.Log("isClimbing = true");
        isClimbing = true;
        _rb.gravityScale = 0;
        _rb.excludeLayers = LayerMask.GetMask("Floor");
        _direction = 0;
    }

    void DisableClimbing()
    {
        isClimbing = false;
        _rb.rotation += 180; // guaranteed rotation upon stopping climbing, as to encourage more varied movement
        _rb.gravityScale = 1;
        _rb.excludeLayers = new LayerMask();
        _verticalDirection = 0;
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
        StartCoroutine(IdleWait());
    }

    IEnumerator IdleWait()
    {
        yield return new WaitForSeconds(Random.Range(0.0f, 0.3f));
        StartCoroutine(IdleMove());
    }

    IEnumerator ClimbUp()
    {
        _verticalDirection = 1.0f;
        yield return new WaitUntil(() => !isClimbing);
    }
    
    IEnumerator ClimbDown()
    {
        _verticalDirection = -1.0f;
        yield return new WaitForSeconds(1 / defaultSpeed); // same as player climbing down. inverse function according to speed, so the faster they climb the sooner collision re-enables
        _rb.excludeLayers = new LayerMask();
        yield return new WaitUntil(() => !isClimbing);
    }
}
