using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    private Rigidbody2D _rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        ResetBall();
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Paddle")) return;
        if (!Mathf.Approximately(other.rigidbody.linearVelocityX, 0.0f)) // changed so it can start at steeper angles all the time
        {
            // A = _rb.linearVelocity = Ball
            // B = other.Rigidbody2D.linearVelocityX = Paddle

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("ScoreZone"))
        {
            GameBehavior.Instance.LoseLife();
            if (GameBehavior.Instance.Lives > 0)
            {
                ResetBall();
            }
            else
            {
                GameBehavior.Instance.GameOver();
            }
        }
    }
    
    public void ResetBall()
    {
        _rb.linearVelocity = Vector2.zero;
        transform.position = new Vector3(0.0f, -1.6f, 0.0f);

        // compute random 2D direction, and normalize in range -1...1
        Vector2 direction = new Vector2(
            GetNonZeroRandomFloat(),
            GetNonZeroRandomFloat()
        ).normalized;

        // apply force to noramlized vector as an impulse, which behaves as a PING
        _rb.AddForce(direction * GameBehavior.Instance.InitBallForce, ForceMode2D.Impulse);
    }
    
    private float GetNonZeroRandomFloat(float min = -1.0f, float max = 1.0f)
    {
        float num;

        do
        {
            num = Random.Range(min, max);
        } while (Mathf.RoundToInt(num) == 0); // changed condition so the angles always start more steep

        return num;
    }
}
