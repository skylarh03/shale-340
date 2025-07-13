using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ListHighScores : MonoBehaviour
{
    [SerializeField] private HighScore _highScores;
    
    [Header("UI Objects")]
    [SerializeField] private List<TMP_Text> _names;
    [SerializeField] private List<TMP_Text> _scores;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _highScores = GameBehavior.Instance.HighScoreManager.GetHighScores();

        for (int i = 0; i < 5; i++)
        {
            _names[i].text = _highScores.ListOfScores[i].PlayerName;
            _scores[i].text = _highScores.ListOfScores[i].Score.ToString();
        }
    }
}
