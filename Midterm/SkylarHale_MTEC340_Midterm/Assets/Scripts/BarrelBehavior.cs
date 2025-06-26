using System.Collections;
using UnityEngine;

public class BarrelBehavior : MonoBehaviour
{
    public float barrelSpeedX; // for normal side-to-side movement
    public float barrelSpeedY; // for when the barrel happens to go down a ladder

    public float ladderDescendChance = 50.0f; // percent chance of going down a ladder
    
    private float _directionX = 1.0f;

    [SerializeField] private bool _goingDownLadder = false;

    private Rigidbody2D _rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        barrelSpeedY = barrelSpeedX / 2;

        _directionX = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(_rb.linearVelocity);
    }

    void FixedUpdate()
    {
        if (!_goingDownLadder) _rb.linearVelocityX = _directionX * barrelSpeedX;
        else
        {
            _rb.linearVelocityX = 0;
            _rb.linearVelocityY = -barrelSpeedY; // negative to make it go down
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (_goingDownLadder)
        {
            if (collision.gameObject.CompareTag("Floor"))
            {
                _goingDownLadder = false;
                _directionX *= -1.0f;
            }
        }
        else
        {
            if (collision.gameObject.CompareTag("World Border")) _directionX *= -1.0f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        float num = Random.Range(0, 100);
        
        if (other.gameObject.CompareTag("Destroy Barrel")) Destroy(gameObject);
        
        else if (other.gameObject.CompareTag("Barrel Descend") && num < ladderDescendChance) StartCoroutine(GoDownLadder());
    }

    private IEnumerator GoDownLadder()
    {
        _goingDownLadder = true;
        _rb.excludeLayers = LayerMask.GetMask("Floor");
        
        // the faster the barrel speed is, the shorter time it takes to re-enable collision
        // the slower the barrel speed is, the longer time it takes to re-enable collision
        yield return new WaitForSeconds(1.0f / barrelSpeedY);
        _rb.excludeLayers = new LayerMask(); // allow for floor collision again after barrel descends through the upper floor
    }
}
