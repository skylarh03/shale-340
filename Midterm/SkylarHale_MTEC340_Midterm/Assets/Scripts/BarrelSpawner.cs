using UnityEngine;
using System.Collections;

public class BarrelSpawner : MonoBehaviour
{
    public float minimumSpawnInterval = 5.0f;
    public float maximumSpawnInterval = 8.0f;
    private float _timeToWait = 0.0f;
    private float _timeElapsedBeforePause = 0.0f;
    
    public GameObject barrelPrefab;
    
    private bool isWaiting = false;

    void Start()
    {
        Instantiate(barrelPrefab, transform).SetActive(true);
    }

    // Update is called once per frame
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
                StartCoroutine(SpawnBarrel(minimumSpawnInterval, maximumSpawnInterval, _timeElapsedBeforePause));
            }
        }
    }

    IEnumerator SpawnBarrel(float minWait, float maxWait, float pauseTimeDifference = 0.0f)
    {
        isWaiting = true;
        //Debug.Log(pauseTimeDifference);

        // only update time to wait if there isn't already a non-zero value stored
        // this way we don't override old wait times by pausing
        if (_timeToWait == 0.0f) _timeToWait = Random.Range(minWait, maxWait) - pauseTimeDifference;

        while (_timeElapsedBeforePause < _timeToWait)
        {
            _timeElapsedBeforePause += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        
        // this only happens if the coroutine doesn't get stopped
        isWaiting = false;
        Instantiate(barrelPrefab, transform).SetActive(true);
        _timeElapsedBeforePause = 0.0f;
        _timeToWait = 0.0f; // reset to generate new value upon next call
    }
}
