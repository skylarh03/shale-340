using UnityEngine;

public class PaddleMovement : MonoBehaviour
{
    public float Speed = 5f;

    public float LimitY = 3.75f;

    public KeyCode upDirection;
    public KeyCode downDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float movement = 0f;

        // Check if given keycode is currently pressed
        if (Input.GetKey(upDirection))
        {
            // Update position by adding two vectors
            movement += Speed;
        }

        if (Input.GetKey(downDirection))
        {
            // Update position by adding two vectors
            movement -= Speed;
        }

        Vector3 newPosition = transform.position + new Vector3(0f, movement, 0f) * Time.deltaTime;

        newPosition.y = Mathf.Clamp(newPosition.y, -LimitY, LimitY);

        transform.position = newPosition;

        // transform.Translate(movementVector);
    }
}
