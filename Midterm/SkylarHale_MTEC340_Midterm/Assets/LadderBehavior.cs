using UnityEngine;

public class LadderBehavior : MonoBehaviour
{
    public GameObject heightCapCollision;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            EnableHeightCap();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            DisableHeightCap();
        }
    }
    
    public void EnableHeightCap()
    {
        heightCapCollision.SetActive(true);
    }

    public void DisableHeightCap()
    {
        heightCapCollision.SetActive(false);
    }
}
