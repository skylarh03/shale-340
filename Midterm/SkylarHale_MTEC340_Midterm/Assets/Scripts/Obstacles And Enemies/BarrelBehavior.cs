using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BarrelBehavior : MonoBehaviour
{
    public float barrelSpeedX; // for normal side-to-side movement
    public float barrelSpeedY; // for when the barrel happens to go down a ladder

    private float initialSpeed;

    public float ladderDescendChance = 50.0f; // percent chance of going down a ladder
    
    private float _directionX = 1.0f;

    private Vector2 _previousVelocity;
    private bool isPaused = false;

    [SerializeField] private bool _goingDownLadder = false;

    [SerializeField] private GameObject _pointsScored;
    private bool _hasBeenScored = false;

    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private CircleCollider2D _circleCollider;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _circleCollider = GetComponent<CircleCollider2D>();

        initialSpeed = barrelSpeedX;
        
        barrelSpeedY = barrelSpeedX / 2;

        _directionX = 1.0f;
        
        // upon every new barrel spawn, add it to the GM barrel list to keep track of all active barrels
        GameBehavior.Instance._activeBarrels.Add(gameObject);
    }

    void Update()
    {
        // check state
        if (GameBehavior.Instance.CurrentState == Utilities.GameState.Pause)
        {
            if (!isPaused)
            {
                isPaused = true; // set flag
                
                // store velocity for use when returning to play state
                _previousVelocity = _rb.linearVelocity;
                _rb.linearVelocity = Vector2.zero;
                _rb.gravityScale = 0;
            }
        }
        else if (GameBehavior.Instance.CurrentState == Utilities.GameState.Play)
        {
            if (isPaused)
            {
                isPaused = false; // set flag
                _rb.linearVelocity = _previousVelocity;
                _rb.gravityScale = _goingDownLadder ? 0 : 1;
            }
        }
        else if (GameBehavior.Instance.CurrentState == Utilities.GameState.WinLevel)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.gravityScale = 0;
        }
    }

    void FixedUpdate()
    {
        if (GameBehavior.Instance.CurrentState == Utilities.GameState.Play)
        {
            if (!_goingDownLadder)
            {
                if (!_hasBeenScored) _rb.gravityScale = 1; // fixedupdate still runs after barrel is destroyed by hammer, so this will keep score text in place
                _rb.linearVelocityX = _directionX * barrelSpeedX;
            }
            else
            {
                _rb.linearVelocityX = 0;
                _rb.linearVelocityY = -barrelSpeedY; // negative to make it go down
            }
        }
        else if (GameBehavior.Instance.CurrentState == Utilities.GameState.Death)
        {
            StopAllCoroutines();
            _rb.linearVelocity = Vector2.zero;
            _rb.gravityScale = 0;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) Destroy(gameObject);
        
        if (_goingDownLadder)
        {
            if (collision.gameObject.CompareTag("Ladder Bottom"))
            {
                //Debug.Log("Barrel colliding with a Ladder Bottom object");
                _goingDownLadder = false;
                _directionX *= -1.0f;
            }
        }
        else
        {
            if (collision.gameObject.CompareTag("World Border")) _directionX *= -1.0f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        float num = Random.Range(0, 100);

        if (other.gameObject.CompareTag("Destroy Barrel"))
        {
            GameBehavior.Instance._activeBarrels.Remove(gameObject);
            Destroy(gameObject);
        }
        
        if (other.gameObject.CompareTag("Hammer"))
        {
            _hasBeenScored = true;
            GameBehavior.Instance.ScorePoints();
            StartCoroutine(ShowPointsScoredAfterHammer());
        }
        
        if (other.gameObject.CompareTag("Barrel Descend") && num < ladderDescendChance) StartCoroutine(GoDownLadder());
        
        // show points scored if player jumps over
        if (other.gameObject.CompareTag("Player") && !_hasBeenScored)
        {
            _hasBeenScored = true;
            GameBehavior.Instance.ScorePoints();
            StartCoroutine(ShowPointsScored());
        }
    }

    public void IncreaseSpeed()
    {
        barrelSpeedX += 0.2f;
        barrelSpeedY = barrelSpeedX / 2;
    }

    public void ResetSpeed()
    {
        barrelSpeedX = 3f;
        barrelSpeedY = barrelSpeedX / 2;
    }

    private IEnumerator GoDownLadder()
    {
        _goingDownLadder = true;
        _rb.excludeLayers = LayerMask.GetMask("Floor", "Ladder Height Cap", "Barrel"); // ignores ladder height cap in case it goes down a ladder the player is on
        
        // the faster the barrel speed is, the shorter time it takes to re-enable collision
        // the slower the barrel speed is, the longer time it takes to re-enable collision
        yield return new WaitForSeconds(1.0f / barrelSpeedY);
        _rb.excludeLayers = new LayerMask(); // allow for floor collision again after barrel descends through the upper floor
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
        
        // remove any direction influence so score text says in one place
        _directionX = 0.0f;
        barrelSpeedY = 0.0f;
        _rb.linearVelocityY = 0.0f;
        
        StartCoroutine(ShowPointsScored());
        
        yield return new WaitForSeconds(0.6f);
        GameBehavior.Instance._activeBarrels.Remove(gameObject);
        Destroy(gameObject);
    }
}
