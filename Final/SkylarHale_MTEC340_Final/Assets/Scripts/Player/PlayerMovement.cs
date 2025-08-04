using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Attributes")] 
    [SerializeField] private float _movementSpeed = 5.0f;

    private float _horizontalDirection = 0.0f;
    private float _verticalDirection = 0.0f;

    private Rigidbody _rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _verticalDirection = Input.GetAxis("Vertical");
        _horizontalDirection = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(_horizontalDirection * _movementSpeed, 0, _verticalDirection * _movementSpeed);
        
        Mathf.Clamp(movement.magnitude, 0, 5);
        
        _rb.MovePosition(_rb.position + (movement * Time.deltaTime));
    }
}
