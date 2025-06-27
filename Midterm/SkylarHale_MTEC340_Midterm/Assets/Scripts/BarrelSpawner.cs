using UnityEngine;
using System.Collections;

public class BarrelSpawner : MonoBehaviour
{
    public float minimumSpawnInterval = 5.0f;
    public float maximumSpawnInterval = 8.0f;
    
    public GameObject barrelPrefab;
    
    private bool isWaiting = false;

    // Update is called once per frame
    void Update()
    {
        if (!isWaiting)
        {
            StartCoroutine(SpawnBarrel(minimumSpawnInterval, maximumSpawnInterval));
        }
    }

    IEnumerator SpawnBarrel(float minWait, float maxWait)
    {
        isWaiting = true;
        Instantiate(barrelPrefab).SetActive(true);
        
        yield return new WaitForSeconds(Random.Range(minWait, maxWait));
        isWaiting = false;
    }
}
