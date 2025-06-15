using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    [SerializeField] private float _launchForce = 5.0f;
    [SerializeField] private float _paddleInfluence = 0.4f;
    [SerializeField] private float _ballSpeedIncrement = 1.1f;
    
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
        if (!Mathf.Approximately(other.rigidbody.linearVelocityX, 0.0f))
        {
            // A = _rb.linearVelocity = Ball
            // B = other.Rigidbody2D.linearVelocityX = Paddle

            // compute new direction based on a weighted sum, giving different priorities to the ball and the paddle.
            // we use a one-minus to get the total sum of the weights to be 1
            Vector2 direction = _rb.linearVelocity * (1 - _paddleInfluence)
                                + other.rigidbody.linearVelocity * _paddleInfluence;

            // apply new direction while maintaining the incoming magnitude
            _rb.linearVelocity = _rb.linearVelocity.magnitude * direction.normalized;
        }

        // apply a small speed increase
        _rb.linearVelocity *= _ballSpeedIncrement;
        
        // 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("ScoreZone"))
        {
            ResetBall();
        }
    }
    
    private void ResetBall()
    {
        _rb.linearVelocity = Vector2.zero;
        transform.position = new Vector3(0.0f, -1.6f, 0.0f);

        // compute random 2D direction, and normalize in range -1...1
        Vector2 direction = new Vector2(
            GetNonZeroRandomFloat(),
            GetNonZeroRandomFloat()
        ).normalized;

        // apply force to noramlized vector as an impulse, which behaves as a PING
        _rb.AddForce(direction * _launchForce, ForceMode2D.Impulse);
    }
    
    private float GetNonZeroRandomFloat(float min = -1.0f, float max = 1.0f)
    {
        float num;

        do
        {
            num = Random.Range(min, max);
        } while (Mathf.Approximately(num, 0.0f));

        return num;
    }
}
