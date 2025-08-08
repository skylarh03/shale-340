using System.Collections;
using UnityEngine;

public class SnowflakeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _snowflakePrefab;
    [SerializeField] private float _spawnRadius;
    [SerializeField] private float _spawnHeight;

    [SerializeField] private float _minWait;
    [SerializeField] private float _maxWait;

    private bool _isWaiting = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isWaiting) StartCoroutine(SpawnSnowflake());
    }

    IEnumerator SpawnSnowflake()
    {
        _isWaiting = true;
        
        // use Raycasting to spawn a snowflake at a random position within a radius
        // first, create a ray that points at a random angle with its origin at the center of the spawner GameObject
        Vector3 spawnDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        Ray ray = new Ray(transform.position, spawnDirection);
        //Debug.Log(spawnDirection);
        
        // then, determine the distance away from the origin to spawn the snowflake, and get a point along the ray
        // at that distance
        float spawnDistance = Random.Range(0, _spawnRadius);
        Vector3 spawnPosition = ray.GetPoint(spawnDistance);
        //Debug.Log(spawnPosition);
        
        // add random amount to spawnPosition Y to get height variance upon spawning
        spawnPosition.y = Random.Range(spawnPosition.y, spawnPosition.y + _spawnHeight);
        
        // finally, instantiate the snowflake prefab at that newly determined position
        Instantiate(_snowflakePrefab, spawnPosition, Quaternion.identity);
        
        yield return new WaitForSeconds(Random.Range(_minWait, _maxWait));
        _isWaiting = false;
    }
}
