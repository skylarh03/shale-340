using UnityEngine;

public class BallBehavior : MonoBehaviour
{ 
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
                Vector2 direction = _rb.linearVelocity * (1 - GameBehavior.Instance.PaddleInfluence)
                                    + other.rigidbody.linearVelocity * GameBehavior.Instance.PaddleInfluence;

                // apply new direction while maintaining the incoming magnitude
                _rb.linearVelocity = _rb.linearVelocity.magnitude * direction.normalized;
            }

            // apply a small speed increase
            _rb.linearVelocity *= GameBehavior.Instance.BallSpeedIncrement;
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("ScoreZone"))
        {
            GameBehavior.Instance.ScorePoint(transform.position.x < 0 ? 2 : 1);
            ResetBall();
        }
    }

    void ResetBall()
    {
        _rb.linearVelocity = Vector2.zero;
        transform.position = Vector3.zero;

        // compute random 2D direction, and normalize in range -1...1
        Vector2 direction = new Vector2(
            Utilities.GetNonZeroRandomFloat(),
            Utilities.GetNonZeroRandomFloat()
        ).normalized;

        // apply force to noramlized vector as an impulse, which behaves as a PING
        _rb.AddForce(direction * GameBehavior.Instance.InitBallForce, ForceMode2D.Impulse);
    }
}
