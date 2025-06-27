using UnityEngine;

public class LadderBehavior : MonoBehaviour
{
    public GameObject TopHeightCollision;
    public GameObject BottomHeightCollision;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TopHeightCollision.SetActive(true);
            BottomHeightCollision.SetActive(true);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        // if player jumps while colliding with ladder trigger, disable height cap collision
        // 1.5f for player climb speed. not sure how to pass that into here
        if (collision.gameObject.CompareTag("Player") && collision.attachedRigidbody.linearVelocityY > 1.5f)
        {
            TopHeightCollision.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TopHeightCollision.SetActive(false);
            BottomHeightCollision.SetActive(false);
        }
    }
}
