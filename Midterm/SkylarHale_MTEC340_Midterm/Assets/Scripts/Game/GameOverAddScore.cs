using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(AudioSource))]
public class GameOverAddScore : MonoBehaviour
{
    [Header("Input Controls")]
    [SerializeField] private KeyCode _leftDirection;
    [SerializeField] private KeyCode _rightDirection;
    [SerializeField] private KeyCode _upDirection;
    [SerializeField] private KeyCode _downDirection;
    [SerializeField] private KeyCode _selectKey;
    
    [Header("UI Panels")]
    [SerializeField] private GameObject _addScorePanel;
    [SerializeField] private GameObject _navigationPanel;
    
    [Header("Add Score Panel Objects")]
    [SerializeField] private TMP_Text _nameLetterOne;
    [SerializeField] private TMP_Text _nameLetterTwo;
    [SerializeField] private TMP_Text _nameLetterThree;
    [SerializeField] private TMP_Text _scoreAchieved;
    
    [Header("Navigation Panel Objects")]
    [SerializeField] private TMP_Text _scoreDisplay;
    [SerializeField] private GameObject _cursor;

    [Header("UI Audio")] 
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _uiMoveClip;
    [SerializeField] private AudioClip _uiSelectClip;

    private int _currentlySelectedLetter = 1;
    private List<TMP_Text> _nameLetters = new List<TMP_Text>();
    
    private Color _selectedColor = new Color(0.9960785f, 0.9019608f, 0.1058824f);
    
    private string _scoreInfo;

    private bool _isHolding = false;
    private bool _scoreAdded = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        
        // initialize all letters as A, highlight first letter as currently selected
        _nameLetterOne.text = Utilities.Letters[0];
        _nameLetterTwo.text = Utilities.Letters[0];
        _nameLetterThree.text = Utilities.Letters[0];
        
        _nameLetterOne.color = _selectedColor;
        
        _nameLetters.Add(_nameLetterOne);
        _nameLetters.Add(_nameLetterTwo);
        _nameLetters.Add(_nameLetterThree);
        
        // pass achieved score into text
        _scoreAchieved.text = GameBehavior.Instance.PlayerScore.Score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(_rightDirection)) MoveSelectionRight();
        else if (Input.GetKeyDown(_leftDirection)) MoveSelectionLeft();
        else if (Input.GetKeyDown(_upDirection)) RotateCurrentLetter(_upDirection);
        else if (Input.GetKeyDown(_downDirection)) RotateCurrentLetter(_downDirection);
        
        // if any key gets held down, run the corresponding coroutine
        if (!_isHolding)
        {
            if (Input.GetKey(_rightDirection)) StartCoroutine(HoldDownKey(_rightDirection));
            else if (Input.GetKey(_leftDirection)) StartCoroutine(HoldDownKey(_leftDirection));
            else if (Input.GetKey(_upDirection)) StartCoroutine(HoldDownKey(_upDirection));
            else if (Input.GetKey(_downDirection)) StartCoroutine(HoldDownKey(_downDirection));
        }
        
        if (Input.GetKeyDown(_selectKey) && !_scoreAdded) AddScore();
    }

    void MoveSelectionRight()
    {
        // visually de-select previous selection
        _nameLetters[_currentlySelectedLetter - 1].color = Color.white;

        // increment selection
        // if previous selection was rightmost letter, wrap around to the left one
        _currentlySelectedLetter++;
        if (_currentlySelectedLetter > _nameLetters.Count) _currentlySelectedLetter -= _nameLetters.Count;
        
        // highlight new selection
        _nameLetters[_currentlySelectedLetter - 1].color = _selectedColor;
        
        Utilities.PlaySound(_audioSource, _uiMoveClip);
    }

    void MoveSelectionLeft()
    {
        // visually de-select previous selection
        _nameLetters[_currentlySelectedLetter - 1].color = Color.white;

        // decrement selection
        // if previous selection was leftmost letter, wrap around to the right one
        _currentlySelectedLetter--;
        if (_currentlySelectedLetter < 1) _currentlySelectedLetter += _nameLetters.Count;
        
        // highlight new selection
        _nameLetters[_currentlySelectedLetter - 1].color = _selectedColor;
        
        Utilities.PlaySound(_audioSource, _uiMoveClip);
    }

    void RotateCurrentLetter(KeyCode direction)
    {
        // get index value of currently selected letter
        int letterIndex = Utilities.Letters.IndexOf(_nameLetters[_currentlySelectedLetter - 1].text);
        
        // apply changes depending on inputted direction
        // if upward, increment letter index. if this value goes past Z's index, wrap it back around to A
        // for downward, the opposite (decrement letter index, then wrap from A to Z if applicable)
        
        // upward
        if (direction == KeyCode.W)
        {
            letterIndex++;
            if (letterIndex > Utilities.Letters.Count - 1) letterIndex -= Utilities.Letters.Count;
        }
        // downward
        else if (direction == KeyCode.S)
        {
            letterIndex--;
            if (letterIndex < 0) letterIndex += Utilities.Letters.Count;
        }
        //Debug.Log(letterIndex);
        
        // apply new letter to currently selected text
        _nameLetters[_currentlySelectedLetter - 1].text = Utilities.Letters[letterIndex];
        
        Utilities.PlaySound(_audioSource, _uiMoveClip);
    }

    void AddScore()
    {
        string scoreName = _nameLetterOne.text + _nameLetterTwo.text + _nameLetterThree.text;
        string scoreAmount = _scoreAchieved.text;
        _scoreInfo = scoreName + " - " +  scoreAmount;
        
        ScoreInfo scoreAchieved = new ScoreInfo(scoreName, GameBehavior.Instance.PlayerScore.Score);
        GameBehavior.Instance.HighScoreManager.AddScore(scoreAchieved);
        _scoreAdded = true;
        
        // disable add score panel, enable navigation panel and its cursor
        _addScorePanel.SetActive(false);
        
        _navigationPanel.SetActive(true);
        _cursor.SetActive(true);
        
        _scoreDisplay.text = _scoreInfo;
        
        Utilities.PlaySound(_audioSource, _uiSelectClip);
    }

    IEnumerator HoldDownKey(KeyCode direction)
    {
        _isHolding = true;
        
        yield return new WaitForSeconds(0.55f);
        while (Input.GetKey(direction))
        {
            // depending on the direction that's being held, do a different action
            switch (direction)
            {
                case KeyCode.W:
                    RotateCurrentLetter(direction);
                    break;
                case KeyCode.S:
                    RotateCurrentLetter(direction);
                    break;
                case KeyCode.A:
                    MoveSelectionLeft();
                    break;
                case KeyCode.D:
                    MoveSelectionRight();
                    break;
            }

            yield return new WaitForSeconds(0.08f);
        }

        _isHolding = false;
    }
}
