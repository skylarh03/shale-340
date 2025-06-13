using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    // public Vector2 Speed = new (5f, 5f); // don't need the second Vector2 when declaring the vector itself
    [SerializeField] private float _launchForce = 5.0f;
    [SerializeField] private float _paddleInfluence = 0.4f;
    [SerializeField] private float _ballSpeedIncrement = 1.1f;

    private Rigidbody2D _rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        ResetBall();

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
        // Speed.x *= Random.value < 0.5 ? -1 : 1;
        // Speed.y *= Random.value < 0.5 ? -1 : 1;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Paddle"))
        {
            if (!Mathf.Approximately(other.rigidbody.linearVelocityY, 0.0f))
            {
                // A = _rb.linearVelocity = Ball
                // B = other.Rigidbody2D.linearVelocityY = Paddle

                // compute new direction based on a weighted sum, giving different priorities to the ball and the paddle.
                // we use a one-minus to get the total sum of the weights to be 1
                Vector2 direction = _rb.linearVelocity * (1 - _paddleInfluence)
                                    + other.rigidbody.linearVelocity * _paddleInfluence;

                // apply new direction while maintaining the incoming magnitude
                _rb.linearVelocity = _rb.linearVelocity.magnitude * direction.normalized;
            }

            // apply a small speed increase
            _rb.linearVelocity *= _ballSpeedIncrement;
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("ScoreZone"))
        {
            ResetBall();
        }
    }

    void ResetBall()
    {
        _rb.linearVelocity = Vector2.zero;
        transform.position = Vector3.zero;

        // compute random 2D direction, and normalize in range -1...1
        Vector2 direction = new Vector2(
            GetNonZeroRandomFloat(),
            GetNonZeroRandomFloat()
        ).normalized;

        // apply force to noramlized vector as an impulse, which behaves as a PING
        _rb.AddForce(direction * _launchForce, ForceMode2D.Impulse);
    }

    float GetNonZeroRandomFloat(float min = -1.0f, float max = 1.0f)
    {
        float num;

        do
        {
            num = Random.Range(min, max);
        } while (Mathf.Approximately(num, 0.0f));

        return num;
    }
}
