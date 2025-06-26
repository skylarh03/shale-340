using UnityEngine;
using System.Collections;

public class BarrelSpawner : MonoBehaviour
{
    public float minTime = 5.0f;
    public float maxTime = 8.0f;
    
    public GameObject barrelPrefab;
    
    private bool isWaiting = false;

    // Update is called once per frame
    void Update()
    {
        if (!isWaiting)
        {
            StartCoroutine(SpawnBarrel(minTime, maxTime));
        }
    }

    IEnumerator SpawnBarrel(float minWait, float maxWait)
    {
        isWaiting = true;
        Instantiate(barrelPrefab);
        
        yield return new WaitForSeconds(Random.Range(minWait, maxWait));
        isWaiting = false;
    }
}
