using System.Collections;
using UnityEngine;

public class SnowflakeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _snowflakePrefab;
    
    [Space(10)]
    [Header("Spawn Time Range")]
    [SerializeField] private float _minSpawnTime;
    [SerializeField] private float _maxSpawnTime;

    [Space(10)] 
    [Header("Spawn Ranges")] 
    [SerializeField] private float _spawnHorizontalRadius = 45.0f;
    [SerializeField] private float _maxSpawnHeight = 3.0f;

    private bool _isWaiting = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnSnowflake());
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isWaiting) StartCoroutine(SpawnSnowflake());
    }

    IEnumerator SpawnSnowflake()
    {
        _isWaiting = true;

        Vector3 spawnPosition = new Vector3(
            transform.localPosition.x + Random.Range(_spawnHorizontalRadius * -1, _spawnHorizontalRadius),
            transform.localPosition.y + Random.Range(0, _maxSpawnHeight),
            transform.localPosition.z + Random.Range(_spawnHorizontalRadius * -1, _spawnHorizontalRadius));

        Instantiate(_snowflakePrefab, spawnPosition, Quaternion.identity);
        
        yield return new WaitForSeconds(Random.Range(_minSpawnTime, _maxSpawnTime));
        _isWaiting = false;
    }
}
