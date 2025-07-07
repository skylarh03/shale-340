using System.Collections;
using UnityEngine;

public class LadderSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _ladderPrefab;
    private bool isWaiting = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject newLadder = Instantiate(_ladderPrefab, transform);
        newLadder.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isWaiting)
        {
            StartCoroutine(SpawnLadder());
        }
    }

    IEnumerator SpawnLadder()
    {
        isWaiting = true;
        
        yield return new WaitForSeconds(0.52f);
        
        GameObject newLadder = Instantiate(_ladderPrefab, transform);
        newLadder.SetActive(true);
        isWaiting = false;
    }
}
