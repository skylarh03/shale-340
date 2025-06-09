using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    public Vector2 Speed = new (5f, 5f); // don't need the second Vector2 when declaring the vector itself

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // if (Random.value < 0.5f)
        // {
        //     Speed.x *= -1;
        // }
        // if (Random.value < 0.5f)
        // {
        //     Speed.y *= -1;
        // }

        // Shorthand way of doing the above using ternary operators
        // condition ? passing : failing;
        Speed.x *= Random.value < 0.5 ? -1 : 1;
        Speed.y *= Random.value < 0.5 ? -1 : 1;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3)Speed * Time.deltaTime;
    }
}
