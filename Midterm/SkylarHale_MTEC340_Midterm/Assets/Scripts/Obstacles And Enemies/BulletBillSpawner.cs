using System.Collections;
using UnityEngine;

public class BulletBillSpawner : MonoBehaviour
{
    [Header("Spawning Information")]
    [SerializeField] private Utilities.SpawnDirection _spawnDirection;
    
    private Transform _spawnPoint;
    [SerializeField] private Transform _leftSpawn;
    [SerializeField] private Transform _rightSpawn;
    
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _bulletSpawnRate;
    [SerializeField] private bool _isWaiting = false;

    private float _timeToWait;
    private float _timeElapsedBeforePause;

    void Awake()
    {
        if (_spawnDirection == Utilities.SpawnDirection.Left) _spawnPoint = _leftSpawn;
        else _spawnPoint = _rightSpawn;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnBullet(_timeElapsedBeforePause));
    }

    // Update is called once per frame
    void Update()
    {
        if (GameBehavior.Instance.CurrentState != Utilities.GameState.Play)
        {
            StopAllCoroutines();
            _isWaiting = GameBehavior.Instance.CurrentState != Utilities.GameState.Pause;
        }
        else
        {
            if (!_isWaiting) StartCoroutine(SpawnBullet(_timeElapsedBeforePause));
        }
    }

    public void IncreaseSpawnFrequency()
    {
        _bulletSpawnRate *= 0.9f;
    }

    public void ResetSpawnFrequency()
    {
        _bulletSpawnRate = 5.0f;
    }

    public void SetSpawnDirection(Utilities.SpawnDirection spawnDirection)
    {
        _spawnDirection = spawnDirection;
    }

    IEnumerator SpawnBullet(float pauseTimeDifference)
    {
        _isWaiting = true;

        if (_timeToWait == 0.0f) _timeToWait = _bulletSpawnRate - pauseTimeDifference;

        while (_timeElapsedBeforePause < _timeToWait)
        {
            _timeElapsedBeforePause += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        
        Debug.Log("Spawning bullet bill on the " + _spawnDirection);
        var newBulletBill = Instantiate(_bulletPrefab, _spawnPoint);
        newBulletBill.SetActive(true);
        newBulletBill.GetComponent<BulletBillBehavior>().SetInitialSpeed(_spawnDirection);
        
        _isWaiting = false;
        _timeElapsedBeforePause = 0.0f;
        _timeToWait = 0.0f;
    }
}
