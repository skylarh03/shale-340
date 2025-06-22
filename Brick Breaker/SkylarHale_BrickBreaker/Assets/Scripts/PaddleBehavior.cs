using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PaddleMovement : MonoBehaviour
{
    private float _direction = 0.0f;

    [SerializeField] private KeyCode _rightDirection;
    [SerializeField] private KeyCode _leftDirection;

    private Rigidbody2D _rb;

    void Start()
    {
        // get reference to the Rigidbody
        _rb = GetComponent<Rigidbody2D>();

        // initialize attributes in case they weren't set in the inspector
        _rb.linearDamping = 0.0f;
        _rb.angularDamping = 0.0f;
        _rb.gravityScale = 0.0f;
    }

    void FixedUpdate()
    {
        // apply movement using the Linear Velocity attribute of the Rigidbody
        _rb.linearVelocityX = _direction * GameBehavior.Instance.PaddleSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // define direction based on player input
        _direction = 0.0f;

        if (Input.GetKey(_rightDirection)) _direction += 1f;
        if (Input.GetKey(_leftDirection)) _direction -= 1f;
    }
}