using UnityEngine;

public class PaddleBehavior : MonoBehaviour
{
    public float Speed = 5f;
    public float LimitX = 4.35f;

    public KeyCode RightDirection;
    public KeyCode LeftDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float movement = 0f;

        if (Input.GetKey(RightDirection))
        {
            movement += Speed;
        }

        if (Input.GetKey(LeftDirection))
        {
            movement -= Speed;
        }

        Vector3 newPosition = transform.position + new Vector3(movement, 0f, 0f) * Time.deltaTime;

        newPosition.x = Mathf.Clamp(newPosition.x, -LimitX, LimitX);

        transform.position = newPosition;
    }
}
