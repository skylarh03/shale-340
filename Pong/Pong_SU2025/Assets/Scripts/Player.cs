using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
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
            _scoreGUI.text = Score.ToString();
        }
    }
    
    [SerializeField]  private TextMeshProUGUI _scoreGUI;
}
