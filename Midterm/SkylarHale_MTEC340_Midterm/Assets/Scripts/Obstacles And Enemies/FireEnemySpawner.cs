using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

public class FireEnemySpawner : MonoBehaviour
{
    [SerializeField] private bool isWaiting = false;
    
    [SerializeField] private GameObject firePrefab;

    public float minimumSpawnInterval;
    public float maximumSpawnInterval;
    private float _timeToWait = 0.0f;
    private float _timeElapsedBeforePause = 0.0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //StartCoroutine(InitialWait());
    }

    void Update()
    {
        if (GameBehavior.Instance.CurrentState != Utilities.GameState.Play)
        {
            StopAllCoroutines();
            // only set to permanently waiting if not paused (death, win level)
            isWaiting = GameBehavior.Instance.CurrentState != Utilities.GameState.Pause;
        }
        else
        {
            if (!isWaiting)
            {
                StartCoroutine(SpawnFire(minimumSpawnInterval, maximumSpawnInterval, _timeElapsedBeforePause));
            }
        }
    }

    IEnumerator InitialWait()
    {
        yield return new WaitForSeconds(Random.Range(3.0f, 6.0f));
        isWaiting = false;
    }

    IEnumerator SpawnFire(float minWait, float maxWait, float pauseTimeDifference = 0.0f)
    {
        isWaiting = true;

        // only update time to wait if there isn't already a non-zero value stored
        // this way we don't override old wait times by pausing
        if (_timeToWait == 0.0f) _timeToWait = Random.Range(minWait, maxWait) - pauseTimeDifference;

        while (_timeElapsedBeforePause < _timeToWait)
        {
            _timeElapsedBeforePause += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        
        Instantiate(firePrefab, transform).SetActive(true);
        isWaiting = false;
        _timeElapsedBeforePause = 0.0f;
        _timeToWait = 0.0f;
    }
}
