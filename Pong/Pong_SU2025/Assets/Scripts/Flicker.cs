using System.Collections;
using TMPro;
using UnityEngine;

public class Flicker : MonoBehaviour
{
    private TMP_Text _uiText;
    [SerializeField] private float _flickerRate = 0.5f;
    private bool _inCoroutine = false;
    private Coroutine _coroutine;

    void Start()
    {
        _uiText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (!_inCoroutine) 
            _coroutine = StartCoroutine(Blink());

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            StopCoroutine(_coroutine);
            _inCoroutine = false;
        }
    }
    
    IEnumerator Blink()
    {
        _inCoroutine = true;
        
        // toggle boolean
        _uiText.enabled = !_uiText.enabled;

        // yield return null; // skip frame
        yield return new WaitForSeconds(_flickerRate);
        
        _inCoroutine = false;
    }
}
