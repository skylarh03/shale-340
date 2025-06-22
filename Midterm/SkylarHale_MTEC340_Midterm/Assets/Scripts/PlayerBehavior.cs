using Unity.VisualScripting;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private KeyCode leftDirection;
    [SerializeField] private KeyCode rightDirection;
    [SerializeField] private KeyCode upDirection;
    [SerializeField] private KeyCode jumpButton;
    
    [Header("Movement")]
    [SerializeField] private float horizontalSpeed = 5.0f;
    [SerializeField] private float jumpForce;
    [SerializeField] private Vector2 ledgeUpForce = new Vector2(0.05f, 0.1f);
    [SerializeField] private float gravity = 1.0f;
    [SerializeField] private bool isGrounded = false;
    
    private float _direction = 0.0f;
    
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
    }

    // Update is called once per frame
    void Update()
    {
        // define direction based on player input
        _direction = 0.0f;

        if (Input.GetKey(rightDirection)) _direction += 1f;
        if (Input.GetKey(leftDirection)) _direction -= 1f;
        
        // jump logic
        // jump has an initial force, slows down to reach a peak, then falls due to gravity
        // can only happen while grounded
        if (Input.GetKeyDown(jumpButton) && isGrounded)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
        }
    }
    
    void OnCollisionStay2D(Collision2D collision)
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
}
