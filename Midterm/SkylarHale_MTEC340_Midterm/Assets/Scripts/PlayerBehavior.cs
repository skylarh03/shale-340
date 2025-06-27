using System;
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
    
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private bool isClimbing = false;
    
    private float _direction = 0.0f;
    private float _verticalDirection = 0.0f;
    
    private Rigidbody2D _rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        
        // initialize rigidbody values
        _rb.linearDamping = 0.0f;
        _rb.angularDamping = 0.0f;
        _rb.gravityScale = gravity;
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

        if (Input.GetKey(rightDirection)) _direction += 1f;
        if (Input.GetKey(leftDirection)) _direction -= 1f;

        if (Input.GetKey(upDirection)) _verticalDirection += 1f;
        if (Input.GetKey(downDirection)) _verticalDirection -= 1f;
        
        // jump logic
        // jump has an initial force, slows down to reach a peak, then falls due to gravity
        // can only happen while grounded and not climbing
        // if you jump, disable platform collision so you can jump through
        // however, this has to be re-enabled upon falling
        if (Input.GetKeyDown(jumpButton) && isGrounded && !isClimbing)
        {
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
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            _rb.gravityScale = gravity;
        }
    }
    
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
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
                _rb.excludeLayers = new LayerMask();
                //Debug.Log(_rb.linearVelocityY);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ladder") && _verticalDirection != 0.0f) EnableClimbing();

        if (other.gameObject.CompareTag("Ladder Top") && isGrounded && _verticalDirection < 0.0f) EnableClimbing();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ladder"))
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
        isGrounded = false;
        _rb.gravityScale = 0.0f;
        _rb.excludeLayers = LayerMask.GetMask("Floor");
    }
}
