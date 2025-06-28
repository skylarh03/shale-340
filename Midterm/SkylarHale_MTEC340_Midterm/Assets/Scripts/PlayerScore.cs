using UnityEngine;
using TMPro;

public class PlayerScore : MonoBehaviour
{
    // backing variable
    private int _score;

    // getter and setter properties
    public int Score
    {
        get => _score; // same as return score inside {}
        set
        {
            _score = value;
            _scoreText.text = _score.ToString();
        }
    }

    [SerializeField] private TMP_Text _scoreText;
}
