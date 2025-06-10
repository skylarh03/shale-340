using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    public Vector2 Speed = new(5f, 5f);

    public float LimitX;
    public float LimitY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Speed.x *= Random.value < 0.5 ? -1 : 1;
        Speed.y *= Random.value < 0.5 ? -1 : 1;
    }

    // Update is called once per frame
    void Update()
    {
        Speed.x *= Mathf.Abs(transform.position.x) == LimitX ? -1 : 1;
        Speed.y *= transform.position.y == LimitY ? -1 : 1; // not absolute value of y-coordinate here so the ball can fall through the bottom edge of the screen

        Vector3 newPosition = transform.position + (Vector3)Speed * Time.deltaTime;

        newPosition.x = Mathf.Clamp(newPosition.x, -LimitX, LimitX);
        newPosition.y = Mathf.Clamp(newPosition.y, -100, LimitY); // hard-coded 100 so ball can fall beneath the screen

        transform.position = newPosition;
    }
}
