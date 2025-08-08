using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Vector3 movement;
    
    private Rigidbody _rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float deltaX = Input.GetAxis("Horizontal") * _speed;
        float deltaZ = Input.GetAxis("Vertical") * _speed;
        
        movement = new Vector3(deltaX, 0, deltaZ);
        
        movement = Vector3.ClampMagnitude(movement, _speed);

        movement *= Time.deltaTime;
    }

    void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + movement);
    }
}
