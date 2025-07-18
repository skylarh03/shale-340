using System;
using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    private TMP_Text _uiText;
    
    private float _timer;

    public float Timer
    {
        get => _timer;
        set
        {
            _timer = value;
            int minutes = Mathf.FloorToInt(Timer / 60);
            int seconds = Mathf.FloorToInt(Timer % 60);
            _uiText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    void Start()
    {
        _uiText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        Timer += Time.deltaTime;
    }
}
