using UnityEngine;
using System.Collections;

public class FireEnemyBehavior : MonoBehaviour
{
    [SerializeField] private int _lifetime = 120;
    [SerializeField] private GameObject _pointsScored;
    
    [Header("Lines of Sight")]
    [SerializeField] private GameObject lineOfSightRight;
    [SerializeField] private GameObject lineOfSightLeft;
    
    [Header("Movement Parameters")]
    public float defaultSpeed = 1.0f;
    public float chaseSpeed = 1.75f;
    private float currentSpeed;

    private float _direction = 0.0f;
    private float _verticalDirection = 0.0f;
    
    [Header("Conditionals")]
    [SerializeField] private bool doesSeePlayer = false;

    [SerializeField] private bool isChasing = false;
    [SerializeField] private bool isClimbing = false;
    [SerializeField] private bool isAlive = true;
    
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private CircleCollider2D _circleCollider;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _circleCollider = GetComponent<CircleCollider2D>();
        _rb.linearDamping = 0;
        _rb.angularDamping = 0;
        _rb.gravityScale = 1;
        
        GameBehavior.Instance.ActiveFireEnemies.Add(gameObject);
        
        StartCoroutine(IdleMove());
        StartCoroutine(DecreaseLifetime());
    }

    void Update()
    {
        // if fire sees player, begin chasing
        if (doesSeePlayer && !isChasing)
        {
            isChasing = true;
            isClimbing = false;
            _verticalDirection = 0.0f;
            StopAllCoroutines();
            StartCoroutine(DecreaseLifetime()); // this one should always be running until enemy despawns
        }

        if (GameBehavior.Instance.CurrentState != Utilities.GameState.Play)
        {
            _direction = 0.0f;
            _verticalDirection = 0.0f;
            _rb.linearVelocity = Vector2.zero;
            _pointsScored.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if (GameBehavior.Instance.CurrentState == Utilities.GameState.Play)
        {
            _rb.linearVelocityX = currentSpeed * _direction;
            if (isClimbing) _rb.linearVelocityY = defaultSpeed * _verticalDirection;
        }
        else if (GameBehavior.Instance.CurrentState == Utilities.GameState.Death)
        {
            StopAllCoroutines();
            _rb.linearVelocity =  Vector2.zero;
            _rb.gravityScale = 0;
        }
    }

    // this only runs if the enemy is climbing down a ladder and reaches the bottom
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor") && isClimbing && _verticalDirection < 0.0f)
        {
            DisableClimbing();
            StopAllCoroutines();
            StartCoroutine(IdleMove());
            StartCoroutine(DecreaseLifetime());
        }
    }
    
    // if player enters of line of sight (which is a child of the fire enemy GameObject), fire sees player
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            doesSeePlayer = true;
            //_direction = 0.0f;
            currentSpeed = chaseSpeed;
            
            // display points scored if player is above fire enemy
            
        }

        if (other.gameObject.CompareTag("Hammer"))
        {
            GameBehavior.Instance.ScorePoints();
            StartCoroutine(ShowPointsScoredAfterHammer());
        }
    }
    
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.transform.position.x < _rb.position.x) _direction = -1.0f;
            else if  (other.transform.position.x > _rb.position.x) _direction = 1.0f;
        }
        
        // can only climb if not chasing
        if (other.gameObject.CompareTag("Ladder") && !isChasing)
        {
            // if enemy is not currently climbing and is colliding with a ladder, set a 1% chance for every frame colliding to override all current movement and start climbing the ladder
            if (!isClimbing && Random.Range(0.0f, 1.0f) < 0.01)
            {
                EnableClimbing();
                StopAllCoroutines();
                StartCoroutine(ClimbUp());
                StartCoroutine(DecreaseLifetime());
            }
        }

        if (other.gameObject.CompareTag("Ladder Top") && !isChasing)
        {
            if (!isClimbing && Random.Range(0.0f, 1.0f) < 0.01)
            {
                EnableClimbing();
                StopAllCoroutines();
                StartCoroutine(ClimbDown());
                StartCoroutine(DecreaseLifetime());
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
            StartCoroutine(DecreaseLifetime());
        }

        if (other.gameObject.CompareTag("Player"))
        {
            // Debug.Log(other.attachedRigidbody.position);
            // Debug.Log(_rb.position);
            
            // if player is above fire enemy, add to score
            // since there are multiple triggers already, just opted for checking positions instead
            if (Mathf.Abs(other.attachedRigidbody.position.x - _rb.position.x) < 1.0f &&
                other.attachedRigidbody.position.y > _rb.position.y)
            {
                //Debug.Log("player jumped over fire enemy");
                GameBehavior.Instance.ScorePoints();
                StartCoroutine(ShowPointsScored());
            }
            
            StartCoroutine(EndChase());
        }
    }

    void EnableClimbing()
    {
        //Debug.Log("isClimbing = true");
        isClimbing = true;
        _rb.gravityScale = 0;
        _rb.excludeLayers = LayerMask.GetMask("Floor", "Barrel", "Fire Enemy");
        _direction = 0;
    }

    void DisableClimbing()
    {
        isClimbing = false;
        _rb.gravityScale = 1;
        _rb.excludeLayers = LayerMask.GetMask("Barrel", "Fire Enemy");
        _verticalDirection = 0;
    }

    IEnumerator IdleMove()
    {
        // determine rotation of enemy before moving
        // only move line of sight if direction actually changes
        // faking rotation by having two different triggers for line of sight and just alternating which one is enabled based on movement direction
        if (Random.Range(0.0f, 1.0f) < 0.5)
        {
            _direction = -1.0f;
            lineOfSightLeft.SetActive(true);
            lineOfSightRight.SetActive(false);
        }
        else
        {
            _direction = 1.0f;
            lineOfSightLeft.SetActive(false);
            lineOfSightRight.SetActive(true);
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

    IEnumerator EndChase()
    {
        // enemy keeps moving in current direction for set amount of time, then waits before going back to idle movement
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        
        doesSeePlayer = false;
        isChasing = false;
        _direction = 0.0f; // reset direction before triggering wait
        StartCoroutine(IdleWaitLong());
    }

    IEnumerator IdleWait()
    {
        yield return new WaitForSeconds(Random.Range(0.0f, 0.3f));
        StartCoroutine(IdleMove());
    }
    
    IEnumerator IdleWaitLong()
    {
        yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
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
        _rb.excludeLayers = LayerMask.GetMask("Barrel", "Fire Enemy");
        yield return new WaitUntil(() => !isClimbing);
    }

    IEnumerator DecreaseLifetime()
    {
        while (isAlive)
        {
            _lifetime--;
            if (_lifetime <= 0)
            {
                isAlive = false;
                GameBehavior.Instance.ActiveFireEnemies.Remove(gameObject);
                Destroy(gameObject);
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }
    
    IEnumerator ShowPointsScored()
    {
        _pointsScored.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        _pointsScored.SetActive(false);
    }

    IEnumerator ShowPointsScoredAfterHammer()
    {
        // visually kills barrel, but doesn't actually destroy gameobject so we can still show score text
        _sr.enabled = false;
        _circleCollider.enabled = false;
        _rb.excludeLayers = LayerMask.GetMask("Default");
        _rb.gravityScale = 0.0f;
        _direction = 0.0f; // remove any direction influence so score text says in one place
        _verticalDirection = 0.0f;
        
        StartCoroutine(ShowPointsScored());
        
        yield return new WaitForSeconds(0.6f);
        GameBehavior.Instance.ActiveFireEnemies.Remove(gameObject);
        Destroy(gameObject);
    }
}
