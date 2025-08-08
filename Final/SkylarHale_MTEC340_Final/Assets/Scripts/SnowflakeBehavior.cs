using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SnowflakeBehavior : MonoBehaviour
{
    [SerializeField] private float _fallSpeed;

    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Material _specialMaterial;
    
    private Rigidbody _rb;
    private MeshRenderer _mr;

    private bool _isSpecial = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        
        _fallSpeed *= Random.Range(0.8f, 1.2f);
        
        _rb.AddForce(new Vector3(0, _fallSpeed * -1, 0), ForceMode.Impulse);
        
        _mr = GetComponent<MeshRenderer>();
        
        // determine if snowflake is a special one or not
        // if it is, it will change the root and mode of the music, as well as change the rendered material
        // to indicate that it's special. otherwise, keep the default material
        if (Random.Range(0.0f, 100.0f) < 2f)
        {
            _mr.material = _specialMaterial;
            _isSpecial = true;
        }
        else _mr.material = _defaultMaterial;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Ground"))
        {
            // mode and root changing functionality
            if (_isSpecial)
            {
                // get a new random root and mode
                Utilities.Root newRoot = (Utilities.Root)Random.Range(0, Enum.GetValues(typeof(Utilities.Root)).Length);
                Utilities.Mode newMode = (Utilities.Mode)Random.Range(0, Enum.GetValues(typeof(Utilities.Mode)).Length);
                Debug.Log(newRoot);
                Debug.Log(newMode);

                // set FMOD parameters according to newly generated root and mode
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Current Root", (int)newRoot);
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Current Mode", (int)newMode);
            }
            
            Destroy(gameObject);
        }
    }
}
