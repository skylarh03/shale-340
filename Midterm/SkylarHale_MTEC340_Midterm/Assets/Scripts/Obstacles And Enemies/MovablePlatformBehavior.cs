using System;
using UnityEngine;

public class MovablePlatformBehavior : MonoBehaviour
{
    [SerializeField] private Utilities.PlatformMovementAxis _movementAxis;
    
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _oppositeSideDistance;

    private float _boundaryOne;
    private float _boundaryTwo;
    
    private Rigidbody2D _rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        
        // depending on the movement axis, apply an initial force in the corresponding direction
        // also determine boundaries dependent on axis
        if (_movementAxis == Utilities.PlatformMovementAxis.Vertical)
        {
            // vertically moving platforms always start at the bottom, so they're always moving up initially
            _rb.linearVelocityY = _moveSpeed;
            _rb.constraints = RigidbodyConstraints2D.FreezePositionX;

            _boundaryOne = _rb.position.y;
            _boundaryTwo = _rb.position.y +  _oppositeSideDistance;
        }
        else if (_movementAxis == Utilities.PlatformMovementAxis.Horizontal)
        {
            // horizontally moving platforms always start on the left, so they always move right initially
            _rb.linearVelocityX = _moveSpeed;
            _rb.constraints = RigidbodyConstraints2D.FreezePositionY;
            
            _boundaryOne = _rb.position.x;
            _boundaryTwo = _rb.position.x +  _oppositeSideDistance;
        }
        
        _rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        if (_movementAxis == Utilities.PlatformMovementAxis.Vertical)
        {
            // incorporate the platform boundaries
            if (_rb.position.y > _boundaryTwo) _rb.linearVelocityY = -_moveSpeed;
            if (_rb.position.y < _boundaryOne)  _rb.linearVelocityY = _moveSpeed;
        }
        else if (_movementAxis == Utilities.PlatformMovementAxis.Horizontal)
        {
            // incorporate the platform boundaries
            if (_rb.position.x > _boundaryTwo) _rb.linearVelocityX = -_moveSpeed;
            if (_rb.position.x < _boundaryOne)  _rb.linearVelocityX = _moveSpeed;
        }
    }
}
