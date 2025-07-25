using System.Collections.Generic;
using UnityEngine;

public class LevelEnvironment : MonoBehaviour
{
    // stores information about where spawner/powerup objects are in the level to make it
    // easy to pass this information to GameBehavior
    
    public List<GameObject> PickupLocations;
    public List<GameObject> BarrelSpawnerLocations;
    public List<GameObject> FireEnemySpawnerLocations;
    public List<GameObject> BulletSpawnerLocations;
    public GameObject PlayerSpawnLocation;
}
