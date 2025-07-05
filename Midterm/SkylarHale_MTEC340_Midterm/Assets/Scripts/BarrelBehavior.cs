using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BarrelBehavior : MonoBehaviour
{
    public float barrelSpeedX; // for normal side-to-side movement
    public float barrelSpeedY; // for when the barrel happens to go down a ladder

    public float ladderDescendChance = 50.0f; // percent chance of going down a ladder
    
    private float _directionX = 1.0f;
    private float _previousDirectionX;

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

        barrelSpeedY = barrelSpeedX / 2;

        _directionX = 1.0f;
        
        // upon every new barrel spawn, add it to the GM barrel list to keep track of all active barrels
        GameBehavior.Instance._activeBarrels.Add(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameBehavior.Instance.CurrentState != Utilities.GameState.Play && !GameBehavior.Instance.IsPaused)
        {
            _previousDirectionX = _directionX;
            _directionX = 0.0f;
            _rb.linearVelocity = Vector2.zero;
            _rb.gravityScale = 0.0f;
            _pointsScored.SetActive(false);
            GameBehavior.Instance.IsPaused = true;
            Debug.Log("is paused");
        }
        else if (GameBehavior.Instance.CurrentState == Utilities.GameState.Play && GameBehavior.Instance.IsPaused)
        {
            _directionX = _previousDirectionX;
            //if (_goingDownLadder) _rb.linearVelocityY = -barrelSpeedY;
            GameBehavior.Instance.IsPaused = false;
            _rb.gravityScale = 1.0f;
            Debug.Log(_directionX);
        }
    }

    void FixedUpdate()
    {
        if (GameBehavior.Instance.CurrentState == Utilities.GameState.Play)
        {
            if (!_goingDownLadder) _rb.linearVelocityX = _directionX * barrelSpeedX;
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
        
        if (_goingDownLadder)
        {
            if (collision.gameObject.CompareTag("Floor"))
            {
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

    private IEnumerator GoDownLadder()
    {
        _goingDownLadder = true;
        _rb.excludeLayers = LayerMask.GetMask("Floor", "Ladder Height Cap"); // ignores ladder height cap in case it goes down a ladder the player is on
        
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
