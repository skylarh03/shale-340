using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeScreenManager : MonoBehaviour
{
    [Header("Player Input")]
    [SerializeField] private KeyCode _rightDirection;
    [SerializeField] private KeyCode _leftDirection;
    [SerializeField] private KeyCode _selectKey;
    
    [Header("Cutscene Objects")]
    [SerializeField] private Rigidbody2D _playerSprite;
    [SerializeField] private SpriteRenderer _hideUpgradesPanel;
    [SerializeField] private SpriteRenderer _whiteTransitionScreen;
    
    [Header("UI Objects")]
    [SerializeField] private List<UpgradeOption> _upgradeOptions;
    [SerializeField] private TMP_Text _upgradeDescription;
    private int _currentlySelectedUpgrade = 1;
    
    [Header("Upgrades Information")]
    [SerializeField] private Utilities.Upgrades _upgradeOne;
    [SerializeField] private Utilities.Upgrades _upgradeTwo;
    [SerializeField] private Utilities.Upgrades _upgradeThree;
    [SerializeField] private Utilities.Upgrades _selectedUpgrade;
    
    [Header("Audio")]
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _uiMoveClip;
    [SerializeField] private AudioClip _uiSelectClip;

    private bool _hasSelected = false;
    private bool _isHolding = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        
        StartCoroutine(UpgradeScreenIntro());

        // generate upgrades
        _upgradeOne = Utilities.GenerateRandomUpgrade();
        _upgradeTwo = Utilities.GenerateRandomUpgrade();
        _upgradeThree = Utilities.GenerateRandomUpgrade();
        
        // ensure all upgrades are unique
        while (_upgradeTwo == _upgradeOne || _upgradeTwo ==  _upgradeThree) _upgradeTwo = Utilities.GenerateRandomUpgrade();
        while (_upgradeThree == _upgradeOne || _upgradeThree == _upgradeTwo)  _upgradeThree = Utilities.GenerateRandomUpgrade();
        
        _upgradeOptions[0].SetTitle(_upgradeOne);
        _upgradeOptions[1].SetTitle(_upgradeTwo);
        _upgradeOptions[2].SetTitle(_upgradeThree);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameBehavior.Instance.CurrentState == Utilities.GameState.UpgradeScreen)
        {
            if (Input.GetKeyDown(_selectKey) && !_hasSelected)
            {
                SelectUpgrade();
                StartCoroutine(UpgradeScreenOutro());
            }

            if (Input.GetKeyDown(_rightDirection)) MoveSelectionRight();
            else if (Input.GetKeyDown(_leftDirection)) MoveSelectionLeft();

            if (Input.GetKey(_rightDirection) && !_isHolding)
            {
                _isHolding = true;
                StartCoroutine(HoldSelectionRight());
            }
            else if (Input.GetKey(_leftDirection) && !_isHolding)
            {
                _isHolding = true;
                StartCoroutine(HoldSelectionLeft());
            }

            if (Input.GetKeyUp(_rightDirection) || Input.GetKeyUp(_leftDirection))
            {
                _isHolding = false;
                StopAllCoroutines();
            }
        }
    }

    void MoveSelectionRight()
    {
        // visually de-select previous option
        _upgradeOptions[_currentlySelectedUpgrade - 1].DeselectOption();
        
        // move selection
        // if all the way to the right, wrap around
        _currentlySelectedUpgrade++;
        if (_currentlySelectedUpgrade > _upgradeOptions.Count) _currentlySelectedUpgrade = 1;
        
        // visually select new current option
        //Debug.Log(_currentlySelectedUpgrade);
        _upgradeOptions[_currentlySelectedUpgrade - 1].SelectOption();

        // update description
        switch (_currentlySelectedUpgrade)
        {
            case 1:
                _upgradeDescription.text = Utilities.UpgradeDescriptions[(int)_upgradeOne];
                break;
            case 2:
                _upgradeDescription.text = Utilities.UpgradeDescriptions[(int)_upgradeTwo];
                break;
            case 3:
                _upgradeDescription.text = Utilities.UpgradeDescriptions[(int)_upgradeThree];
                break;
        }
        
        Utilities.PlaySound(_audioSource, _uiMoveClip);
    }

    void MoveSelectionLeft()
    {
        // visually de-select previous option
        _upgradeOptions[_currentlySelectedUpgrade - 1].DeselectOption();
        
        // move selection
        // if all the way to the left, wrap around
        _currentlySelectedUpgrade--;
        if (_currentlySelectedUpgrade < 1) _currentlySelectedUpgrade = _upgradeOptions.Count;
        
        // visually select new current option
        //Debug.Log(_currentlySelectedUpgrade);
        _upgradeOptions[_currentlySelectedUpgrade - 1].SelectOption();
        
        // update description
        switch (_currentlySelectedUpgrade)
        {
            case 1:
                _upgradeDescription.text = Utilities.UpgradeDescriptions[(int)_upgradeOne];
                break;
            case 2:
                _upgradeDescription.text = Utilities.UpgradeDescriptions[(int)_upgradeTwo];
                break;
            case 3:
                _upgradeDescription.text = Utilities.UpgradeDescriptions[(int)_upgradeThree];
                break;
        }
        
        Utilities.PlaySound(_audioSource, _uiMoveClip);
    }

    void SelectUpgrade()
    {
        _hasSelected = true;
        
        switch (_currentlySelectedUpgrade)
        {
            case 1:
                _selectedUpgrade = _upgradeOne;
                break;
            case 2:
                _selectedUpgrade = _upgradeTwo;
                break;
            case 3:
                _selectedUpgrade = _upgradeThree;
                break;
        }

        // apply logic based on selected upgrade
        // remaining cases: 0 - blue shell (dash), 4 - super leaf (glide), 6 - boomerang flower, 7 - ice flower
        switch ((int)_selectedUpgrade)
        {
            case 0:
                GameBehavior.Instance.ApplyBlueShell();
                break;
            case 1:
                GameBehavior.Instance.ApplyDashPepper();
                break;
            case 2:
                GameBehavior.Instance.ApplySuperMushroom();
                break;
            case 3:
                GameBehavior.Instance.ApplyGoombaShoe();
                break;
            case 4:
                GameBehavior.Instance.ApplySuperLeaf();
                break;
            case 5:
                GameBehavior.Instance.ApplySuperHammer();
                break;
            case 6:
                GameBehavior.Instance.ApplyBoomerangFlower();
                break;
            case 7:
                GameBehavior.Instance.ApplyIceFlower();
                break;
        }
        
        Utilities.PlaySound(_audioSource, _uiSelectClip);
    }

    IEnumerator HoldSelectionRight()
    {
        yield return new WaitForSeconds(0.55f);

        while (Input.GetKey(_rightDirection))
        {
            MoveSelectionRight();
            yield return new WaitForSeconds(0.08f);
        }
    }

    IEnumerator HoldSelectionLeft()
    {
        yield return new WaitForSeconds(0.55f);

        while (Input.GetKey(_leftDirection))
        {
            MoveSelectionLeft();
            yield return new WaitForSeconds(0.08f);
        }
    }

    // cutscene where player sprite climbs ladder and upgrades are revealed
    IEnumerator UpgradeScreenIntro()
    {
        _playerSprite.linearVelocityY = 2.0f;
        StartCoroutine(RevealUpgrades());
        
        // wait until player sprite climbs to middle of screen
        while (_playerSprite.position.y < 0.0f)
        {
            yield return new WaitForSeconds(0.05f);
        }
        
        _playerSprite.linearVelocityY = 0.0f;

        Debug.Log((int)_upgradeOne);
        _upgradeDescription.text = Utilities.UpgradeDescriptions[(int)_upgradeOne];

        _upgradeOptions[_currentlySelectedUpgrade - 1].SelectOption();
        GameBehavior.Instance.CurrentState = Utilities.GameState.UpgradeScreen;
    }

    // reveal upgrades via a fade in
    IEnumerator RevealUpgrades()
    {
        yield return new WaitForSeconds(1.5f);

        var timer = 0.0f;
        while (timer < 2.0f)
        {
            timer += 0.01f;
            _hideUpgradesPanel.color -= new Color(0, 0, 0, 0.005f);
            yield return new WaitForSeconds(0.01f);
        }
    }
    
    // cutscene where player sprite climbs to top of screen, overall game fades to white
    IEnumerator UpgradeScreenOutro()
    {
        GameBehavior.Instance.CurrentState = Utilities.GameState.Cutscene;
        _playerSprite.linearVelocityY = 2.0f;
        StartCoroutine(FadeToWhite());
        yield return new WaitForSeconds(4.0f);
        SceneManager.UnloadSceneAsync("UpgradeScreen");
        GameBehavior.Instance.NextLevel();
    }

    // fade screen to white
    IEnumerator FadeToWhite()
    {
        var timer = 0.0f;
        while (timer < 2.5f)
        {
            timer += 0.01f;
            _whiteTransitionScreen.color += new Color(0, 0, 0, 0.004f);
            GameBehavior.Instance.Music.volume -= 0.004f;
            yield return new WaitForSeconds(0.01f);
        }

        GameBehavior.Instance.Music.Stop();
        GameBehavior.Instance.Music.volume = 1.0f;
        yield return new WaitForSeconds(1.0f);
    }
}
