using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    
    public AK.Wwise.Event[] SetRootEvents = new AK.Wwise.Event[12];
    public AK.Wwise.Event[] SetModeEvents = new AK.Wwise.Event[7];

    public AK.Wwise.Event SnowNoteEvent;

    [HideInInspector] private Utilities.Root CurrentRoot;
    [HideInInspector] private Utilities.Mode CurrentMode;

    void Awake()
    {
        // singleton pattern initializer
        if (Instance != null && Instance != this)
            Destroy(this);
        else
        {
            // assign Music Manager if none exists
            Instance = this;
            
            // make sure object owning the Music Manager isn't destroyed when
            // exiting the scene
            DontDestroyOnLoad(gameObject);
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentRoot = (Utilities.Root)Random.Range(0, Enum.GetValues(typeof(Utilities.Root)).Length);
        CurrentMode = (Utilities.Mode)Random.Range(0, Enum.GetValues(typeof(Utilities.Mode)).Length);
        
        Debug.Log(CurrentRoot);
        Debug.Log(CurrentMode);

        SetRootEvents[(int)CurrentRoot].Post(gameObject);
        SetModeEvents[(int)CurrentMode].Post(gameObject);
    }

    public void PlaySnowNote(GameObject snowflake)
    {
        SnowNoteEvent.Post(snowflake);
    }

    public Utilities.Root GetCurrentRoot()
    {
        return CurrentRoot;
    }

    public Utilities.Mode GetCurrentMode()
    {
        return CurrentMode;
    }
}
