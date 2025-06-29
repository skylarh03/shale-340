using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private KeyCode leftDirection;
    [SerializeField] private KeyCode rightDirection;
    [SerializeField] private KeyCode upDirection;
    [SerializeField] private KeyCode downDirection;
    [SerializeField] private KeyCode jumpButton;
    
    [Header("Movement")]
    [SerializeField] private float horizontalSpeed = 5.0f;
    [SerializeField] private float climbSpeed = 5.0f;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity = 1.0f;
    private Vector2 _initialPosition;
    
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private bool isClimbing = false;
    [SerializeField] private bool isJumping = false;

    [Header("Hammer Powerup")] 
    [SerializeField] private bool isHammer = false;
    [SerializeField] private GameObject hammerLeft;
    [SerializeField] private GameObject hammerRight;
    
    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;

    private bool isAlive = true;
    
    private float _direction = 0.0f;
    private float _verticalDirection = 0.0f;
    
    private Rigidbody2D _rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
        
        // initialize rigidbody values
        _rb.linearDamping = 0.0f;
        _rb.angularDamping = 0.0f;
        _rb.gravityScale = gravity;
        
        _initialPosition = _rb.position;
    }

    void FixedUpdate()
    {
        // apply movement using the Linear Velocity attribute of the Rigidbody
        // only allow horizontal input if player isn't climbing
        // if they are, then allow both axes
        _rb.linearVelocityX = _direction * horizontalSpeed;
        if (isClimbing) _rb.linearVelocityY = _verticalDirection * climbSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // define direction based on player input
        _direction = 0.0f;
        _verticalDirection = 0.0f;

        // must be in play state for direction to be applied
        if (GameBehavior.Instance.CurrentState == Utilities.GameState.Play)
        {
            if (Input.GetKey(rightDirection)) _direction += 1f;
            if (Input.GetKey(leftDirection)) _direction -= 1f;

            // only allow vertical input if NOT using hammer
            if (Input.GetKey(upDirection) && !isHammer) _verticalDirection += 1f;
            if (Input.GetKey(downDirection) && !isHammer) _verticalDirection -= 1f;
        }
        
        // jump logic
        // jump has an initial force, slows down to reach a peak, then falls due to gravity
        // can only happen while grounded and not climbing
        // if you jump, disable platform collision so you can jump through
        // however, this has to be re-enabled upon falling
        if (Input.GetKeyDown(jumpButton) && isGrounded && !isClimbing)
        {
            isJumping = true;
            
            // play jump sound, clip is in source in inspector right now
            _audioSource.Play();
            
            // fun little bug happened:
            // if you moved along a sloped collision to climb to the next platform and jumped, you would gain additional
            // vertical velocity, thereby meaning you jumped higher.
            // this is an attempt at capping the initial velocity no matter what happens
            if (Mathf.Approximately(_rb.linearVelocity.y, 0.0f))
            {
                //Debug.Log("Jumped off of flat ground.");
                _rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            }
            else
            {
                // if you're already accelerating vertically, add the difference between the intended jump velocity
                // and your current vertical velocity
                //Debug.Log("Jumped off of sloped collision.");
                _rb.AddForce(Vector3.up * (jumpForce - _rb.linearVelocityY), ForceMode2D.Impulse);
            }
            //Debug.Log(_rb.linearVelocityY);
            
            _rb.excludeLayers = LayerMask.GetMask("Floor");
            isGrounded = false;
        }
        
        // hammer powerup
        // if it's active, change the way the hammer is facing depending on which way the player is facing
        // kind of faking this by having two hammer game objects that will alternate SetActive() status
        if (isHammer)
        {
            if (_direction < 0.0f)
            {
                hammerLeft.SetActive(true);
                hammerRight.SetActive(false);
            }
            else if (_direction > 0.0f)
            {
                hammerLeft.SetActive(false);
                hammerRight.SetActive(true);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            _rb.gravityScale = gravity;
            isJumping = false;
        }
        
        // collide with barrel, trigger death
        if (collision.gameObject.CompareTag("Barrel") || collision.gameObject.CompareTag("Fire Enemy"))
        {
            GameBehavior.Instance.LoseHealth();
            if (GameBehavior.Instance.CurrentState == Utilities.GameState.Death) StartCoroutine(Death());
        }
    }
    
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
            isJumping = false;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        // only ungrounds if the player walks off a ledge and is falling
        if (other.gameObject.CompareTag("Floor") && _rb.linearVelocityY < -1.0f)
        {
            isGrounded = false;
        }
    }

    // to compensate for falling at any distance, there are triggers above all the ground tiles
    // these triggers will re-enable all collision
    // in order to allow for jumping through platforms, this must only happen if the player is falling
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enable Floor Collision"))
        {
            if (_rb.linearVelocityY < -1.0f) // -1.0f so the player doesn't snap through collision at the apex of their jump
            {
                isGrounded = true;
                isJumping = false;
                _rb.excludeLayers = new LayerMask();
                //Debug.Log(_rb.linearVelocityY);
            }
        }
        
        // powerup activation
        if (other.gameObject.CompareTag("Powerup"))
        {
            other.gameObject.SetActive(false);
            isHammer = true;
            StartCoroutine(HammerPowerup());
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ladder") && _verticalDirection != 0.0f) EnableClimbing();

        if (other.gameObject.CompareTag("Ladder Top") && isGrounded && _verticalDirection < 0.0f) EnableClimbing();
        
        // if player is in win level trigger, is not climbing, is currently in play state and has not been detected to have already
        // won the level, move to win level state.
        // this gets rid of active obstacles in level and plays music before allowing player to move to ladders to progress
        // to the next level.
        if (other.gameObject.CompareTag("Win Level Trigger") &&
            GameBehavior.Instance.CurrentState == Utilities.GameState.Play && !isClimbing && !GameBehavior.Instance.HasWonLevel)
        {
            GameBehavior.Instance.CurrentState = Utilities.GameState.WinLevel;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // for some reason this runs ALSO when updating layer overrides in death coroutine?
        // just run this if the player is alive as well i guess
        if (other.gameObject.CompareTag("Ladder") && isAlive)
        {
            isClimbing = false;
            //isGrounded = true;
            _rb.gravityScale = gravity;
            _rb.excludeLayers = new LayerMask();
        }
    }

    private void EnableClimbing()
    {
        isClimbing = true;
        isJumping = false;
        isGrounded = false;
        _rb.gravityScale = 0.0f;
        _rb.excludeLayers = LayerMask.GetMask("Floor");
    }

    public void ResetPlayer()
    {
        isAlive = true;
        _rb.excludeLayers = new LayerMask();
        _rb.gravityScale = gravity;
        _rb.linearVelocity = Vector2.zero;
        _rb.position = _initialPosition;
    }

    IEnumerator HammerPowerup()
    {
        // switch music
        GameBehavior.Instance.Music.resource = GameBehavior.Instance.PowerupMusic;
        GameBehavior.Instance.Music.Play();
        yield return new WaitForSeconds(GameBehavior.Instance.PowerupDuration);
        
        // switch music back
        GameBehavior.Instance.Music.resource = GameBehavior.Instance.LevelMusic;
        GameBehavior.Instance.Music.Play();
        
        isHammer = false;
        
        hammerLeft.SetActive(false);
        hammerRight.SetActive(false);
    }
    
    IEnumerator Death()
    {
        //Debug.Log("player death coroutine called");
        
        // syncing animation to sound, will have a lot of very specific WaitForSeconds() times
        _direction = 0.0f;
        _verticalDirection = 0.0f;
        _rb.linearVelocity = Vector2.zero;
        _rb.gravityScale = 0.0f;

        isAlive = false;
        
        _rb.excludeLayers = LayerMask.GetMask("Floor", "Barrel", "Fire Enemy", "Default", "Ladder", "Ladder Top");

        yield return new WaitForSeconds(0.59f);
        
        for (int i = 0; i < 16; i++)
        {
            _rb.rotation -= 45;
            yield return new WaitForSeconds(0.11f);
        }

        yield return new WaitForSeconds(0.14f);
        _rb.linearVelocityY = -1.5f;
        yield return new WaitForSeconds(1.16f);
        _rb.linearVelocityY = 0.0f;
    }
}