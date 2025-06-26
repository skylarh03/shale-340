using UnityEngine;

public class LadderBehavior : MonoBehaviour
{
    public GameObject HeightCollision;

    void OnTriggerEnter2D(Collider2D collision)
    {
        HeightCollision.SetActive(true);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        // if player jumps while colliding with ladder trigger, disable height cap collision
        // 1.5f for player climb speed. not sure how to pass that into here
        if (collision.gameObject.CompareTag("Player") && collision.attachedRigidbody.linearVelocityY > 1.5f)
        {
            HeightCollision.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (HeightCollision.activeSelf)
        {
            HeightCollision.SetActive(false);
        }
    }
}
