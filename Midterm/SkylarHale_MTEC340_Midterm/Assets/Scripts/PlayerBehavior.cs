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
    [SerializeField] private Vector2 ledgeUpForce = new Vector2(0.05f, 0.1f);
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
        _rb.linearVelocityX = _direction * horizontalSpeed;

        if (isClimbing)
        {
            _rb.linearVelocityY = _verticalDirection * climbSpeed;
        }
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
        // can only happen while grounded
        // if you jump, disable platform collision so you can jump through
        // however, this has to be re-enabled upon falling
        if (Input.GetKeyDown(jumpButton) && isGrounded)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            _rb.excludeLayers = LayerMask.GetMask("Floor", "Wall");
            isGrounded = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // allow sprite to climb up any short jumps in environment
        // only do this if the player is colliding with a wall, is grounded, and is moving
        if (collision.gameObject.CompareTag("Wall") && isGrounded && _direction != 0.0f)
        {
            _rb.MovePosition(new Vector2(_rb.position.x + (ledgeUpForce.x * _direction), _rb.position.y + ledgeUpForce.y));
            //Debug.Log(collision.gameObject.tag);
            //Debug.Log(_direction);
        } 
    }
    
    void OnCollisionStay2D(Collision2D collision)
    {
        
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        // only ungrounds if the player walks off a ledge
        if (other.gameObject.CompareTag("Floor") && _rb.linearVelocityY < 0.0f)
        {
            isGrounded = false;
        }
    }

    // to compensate for falling at any distance, there are triggers above all the ground tiles
    // these triggers will re-enable all collision
    // in order to allow for jumping through platforms, this must only happen if the player's vertical
    // velocity is negative (the player needs to be definitely falling)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enable Floor Collision") && _rb.linearVelocityY < 0.0f)
        {
            isGrounded = true;
            _rb.excludeLayers = new LayerMask();
        }
    }

    // snap to ladder if player is in trigger and hits up key
    // override normal floor collision so you can climb through
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ladder") && !isClimbing)
        {
            // check for vertical input when colliding with ladder trigger
            // only allow down input if player is at top of ladder
            if ((_verticalDirection != 0.0f && _rb.position.y < other.transform.position.y)|| Input.GetKey(downDirection))
            {
                isClimbing = true;
                isGrounded = false;
                _rb.gravityScale = 0;
                _rb.excludeLayers = LayerMask.GetMask("Floor");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ladder"))
        {
            isClimbing = false;
            _rb.gravityScale = gravity;
            _rb.excludeLayers = new LayerMask(); // equivalent of setting it to nothing
            
            // override upward force upon exiting ladder, but only if hitting vertical inputs.
            // because of needing to climb down, the trigger must overlap with walkable area
            // on the upper floor. so, we need to IGNORE this if the player happens to jump
            // in this trigger area
            if (Input.GetKey(upDirection))
            {
                _rb.linearVelocityY = 0.0f;
            }
        }
    }
}
