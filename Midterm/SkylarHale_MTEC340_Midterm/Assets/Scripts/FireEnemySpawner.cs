using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

public class FireEnemySpawner : MonoBehaviour
{
    private bool isWaiting = true;
    
    [SerializeField] private GameObject firePrefab;

    [SerializeField] private float minimumSpawnInterval;
    [SerializeField] private float maximumSpawnInterval;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(InitialWait());
    }

    void Update()
    {
        if (!isWaiting) StartCoroutine(SpawnFire());
    }

    IEnumerator InitialWait()
    {
        yield return new WaitForSeconds(Random.Range(3.0f, 6.0f));
        isWaiting = false;
    }

    IEnumerator SpawnFire()
    {
        Instantiate(firePrefab).SetActive(true);
        isWaiting = true;
        
        yield return new WaitForSeconds(Random.Range(minimumSpawnInterval, maximumSpawnInterval));
        isWaiting = false;
    }
}
