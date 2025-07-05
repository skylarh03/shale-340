using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UINavigator : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> _options;
    [SerializeField] private int _currentOption = 1;
    [SerializeField] private float _cursorMoveDistance = 0.83f;

    [SerializeField] private KeyCode _upDirection;
    [SerializeField] private KeyCode _downDirection;
    [SerializeField] private KeyCode _selectKey;
    
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _uiMoveClip;
    [SerializeField] private AudioClip _uiSelectClip;

    private bool _isHolding = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(_downDirection)) MoveCursorDown();
        else if  (Input.GetKeyDown(_upDirection)) MoveCursorUp();
        else if (Input.GetKeyDown(_selectKey)) SelectOption();

        if (Input.GetKey(_downDirection) && !_isHolding)
        {
            _isHolding = true;
            StartCoroutine(HoldCursorDown());
        }
        else if (Input.GetKey(_upDirection) && !_isHolding)
        {
            _isHolding = true;
            StartCoroutine(HoldCursorUp());
        }

        if (Input.GetKeyUp(_downDirection) || Input.GetKeyUp(_upDirection))
        {
            _isHolding = false;
            StopAllCoroutines();
        }

    }

    void MoveCursorDown()
    {
        // visually de-select previous option by turning the text color to white
        _options[_currentOption - 1].color = Color.white;
        
        // move cursor
        // the cursor is rotated -90 degrees on the z-axis, so use Vector3.right for moving down, and Vector3.left for moving up
        _currentOption++;
        if (_currentOption > _options.Count) {
            _currentOption = 1; // wrap around menu
            transform.Translate(Vector3.left * (_cursorMoveDistance * (_options.Count - 1)));
        }
        else transform.Translate(Vector3.right * _cursorMoveDistance);
        
        // visually select new current option by changing text color
        _options[_currentOption - 1].color = new Color(0.9960785f, 0.9019608f, 0.1058824f);
        
        Utilities.PlaySound(_audioSource, _uiMoveClip);
    }
    
    void MoveCursorUp()
    {
        // visually de-select previous option by turning the text color to white
        _options[_currentOption - 1].color = Color.white;
        
        // move cursor
        // the cursor is rotated -90 degrees on the z-axis, so use Vector3.right for moving down, and Vector3.left for moving up
        _currentOption--;
        if (_currentOption < 1) {
            _currentOption = _options.Count; // wrap around menu
            transform.Translate(Vector3.right * (_cursorMoveDistance * (_options.Count - 1)));
        }
        else transform.Translate(Vector3.left * _cursorMoveDistance);
        
        // visually select new current option by changing text color
        _options[_currentOption - 1].color = new Color(0.9960785f, 0.9019608f, 0.1058824f);
        
        Utilities.PlaySound(_audioSource, _uiMoveClip);
    }

    void SelectOption()
    {
        Utilities.PlaySound(_audioSource, _uiSelectClip);
        if (_options.Count == 2)
        {
            // option-dependent actions
            // 1 - go back to game
            // 2 - quit game
            switch (_currentOption)
            {
                case 1:
                    StartCoroutine(PlayGame());
                    break;
                case 2:
                    StartCoroutine(QuitGame());
                    break;
            }
        }
        else if (_options.Count == 3)
        {
            // option-dependent actions
            // 1 - go back to game
            // 2 - go to main menu
            // 3 - quit game
            switch (_currentOption)
            {
                case 1:
                    StartCoroutine(RestartGame());
                    break;
                case 2:
                    StartCoroutine(GoToMainMenu());
                    break;
                case 3:
                    StartCoroutine(QuitGame());
                    break;
            }
        }
    }

    IEnumerator HoldCursorDown()
    {
        yield return new WaitForSeconds(0.55f);

        while (Input.GetKey(_downDirection))
        {
            MoveCursorDown();
            yield return new WaitForSeconds(0.08f);
        }
    }
    
    IEnumerator HoldCursorUp()
    {
        yield return new WaitForSeconds(0.55f);

        while (Input.GetKey(_upDirection))
        {
            MoveCursorUp();
            yield return new WaitForSeconds(0.08f);
        }
    }

    IEnumerator PlayGame()
    {
        yield return new WaitForSeconds(0.83f);
        GameBehavior.Instance.ResetGame();
    }

    IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(0.83f);
        SceneManager.UnloadSceneAsync("Scenes/GameOver");
        GameBehavior.Instance.ResetGame();
    }

    IEnumerator QuitGame()
    {
        yield return new WaitForSeconds(0.83f);
        EditorApplication.isPlaying = false;
        Application.Quit();
    }

    IEnumerator GoToMainMenu()
    {
        yield return new WaitForSeconds(0.83f);
        SceneManager.UnloadSceneAsync("Scenes/GameOver");
        SceneManager.LoadScene("Scenes/TitleScreen",  LoadSceneMode.Additive);
    }
}
