using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // choose a random type of pickup to spawn
        // 0 - bonus point pickup
        // 1 - health pickup
        // 2 - powerup
        int pickupType = Random.Range(0, 3);
        //Debug.Log(pickupType);

        switch (pickupType)
        {
            case 0:
                Instantiate(GameBehavior.Instance.BonusPointPickup, transform);
                break;
            case 1:
                Instantiate(GameBehavior.Instance.BonusHealthPickup, transform);
                break;
            case 2:
                // spawn powerup based on what is currently unlocked
                Utilities.Powerups powerupToSpawn = GameBehavior.Instance.UnlockedPowerups[Random.Range(0, GameBehavior.Instance.UnlockedPowerups.Count)];
                switch (powerupToSpawn)
                {
                    case Utilities.Powerups.SuperHammer:
                        Instantiate(GameBehavior.Instance.HammerPrefab, transform);
                        break;
                }
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
