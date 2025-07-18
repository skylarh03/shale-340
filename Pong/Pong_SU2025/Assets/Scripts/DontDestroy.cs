using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    // will normally need a singleton pattern

    // Update is called once per frame
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
