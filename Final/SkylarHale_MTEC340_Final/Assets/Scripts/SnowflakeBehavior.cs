using System.Collections;
using UnityEngine;

public class SnowflakeBehavior : MonoBehaviour
{
    [SerializeField] private float _fallSpeed;
    [SerializeField] private AK.Wwise.Event _snowNoteEvent;

    private Rigidbody _rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.linearVelocity = new Vector3(0.0f, _fallSpeed * -1, 0.0f);

        // subtly randomize fall speed so it's not always the same
        _fallSpeed += Random.Range(-0.3f, 0.3f);
    }

    void OnCollisionEnter(Collision collision)
    {
        Utilities.Root currentRoot = MusicManager.Instance.GetCurrentRoot();
        Utilities.Mode currentMode = MusicManager.Instance.GetCurrentMode();
        
        MusicManager.Instance.SetRootEvents[(int)currentRoot].Post(gameObject);
        MusicManager.Instance.SetModeEvents[(int)currentMode].Post(gameObject);
        
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("played snow note");
            StartCoroutine(PlaySnowNote());
            Destroy(gameObject);
        }
    }

    IEnumerator PlaySnowNote()
    {
        yield return null;
        _snowNoteEvent.Post(gameObject);
    }
}
