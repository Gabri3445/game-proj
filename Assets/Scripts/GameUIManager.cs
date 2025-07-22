using Input;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public GameObject gameOverUI;
    public TMP_Text levelText;
    public TMP_Text pointsText;
    public TMP_Text timerText;
    public PopUp namePopup;
    public Camera mainCamera;
    public GameObject pauseUI;
    public Stopwatch stopwatch;
    public GameObject player;


    public GameObject playingUI;
    public TMP_Text checkpointText;
    public TMP_Text livesText;
    private Blur _blur;
    private Character _character;
    private GameInstance _gameInstance;


    private CharacterInput _inputActions;
    private bool _isPaused;
    private MusicManager _musicManager;
    private Rigidbody _playerRb;
    private float _time;


    // ReSharper disable once MemberCanBePrivate.Global
    public static GameUIManager Instance { get; private set; }

    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        _blur = mainCamera.GetComponent<Blur>();
        pauseUI.SetActive(false);
        playingUI.SetActive(true);
        gameOverUI.SetActive(false);
        _inputActions = new CharacterInput();
        _inputActions.Enable();
        _musicManager = GameObject.Find("MusicManagerObject").GetComponent<MusicManager>();
        _character = player.GetComponent<Character>();
        _playerRb = player.GetComponent<Rigidbody>();
        _gameInstance = GameObject.Find("GameInstanceObject").GetComponent<GameInstance>();
        livesText.text = $"Lives remaining: {_gameInstance.livesRemaining}";
    }


    private void Start()
    {
        checkpointText.text = $"Checkpoint {0}/{_gameInstance.TotalCheckpointNumber}";
    }

    private void OnEnable()
    {
        _inputActions.Player.Pause.performed += OnPause;
    }

    private void OnDisable()
    {
        _inputActions.Player.Pause.performed -= OnPause;
        _inputActions.Disable();
    }


    public void OnGameOver()
    {
        _isPaused = true;
        _character.InputActions.Disable();
        PauseGame();
        _time = stopwatch.TimeElapsed;
        IsPaused = true;
        playingUI.SetActive(false);
        pauseUI.SetActive(false);
        gameOverUI.SetActive(true);
        levelText.text = $"Level: {_gameInstance.GetLevelNumber()}";
        timerText.text = $"Time: {stopwatch.TimeToString(_time)}";
        pointsText.text = $"Points: {_gameInstance.points}";
        OnDeath();
    }

    public void OnRetry()
    {
        //TODO, check for lives and bring to checkpoint otherwise bring to level 1
    }

    public void OnSaveLeaderboard()
    {
        namePopup.Open();
    }


    public void OnCheckPointChange(int checkpointNumber)
    {
        checkpointText.text = $"Checkpoint {checkpointNumber}/{_gameInstance.TotalCheckpointNumber}";
    }

    public void OnDeath()
    {
        livesText.text = $"Lives remaining: {_gameInstance.livesRemaining}";
    }


    private void OnPause(InputAction.CallbackContext context)
    {
        if (_isPaused)
        {
            OnResumeButton();
            return;
        }

        _isPaused = true;
        _character.InputActions.Disable();
        PauseGame();
        _time = stopwatch.TimeElapsed;
        IsPaused = true;
        playingUI.SetActive(false);
        pauseUI.SetActive(true);
        _blur.enabled = true; //TODO: could interpolate the blur too
    }

    public void OnBackToMainMenu()
    {
        _musicManager.DisableHighpassInstant();
        _musicManager.DisableLowpassInstant();
        Destroy(_musicManager.gameObject);
        Destroy(gameObject);
        SceneManager.LoadScene(0);
    }

    public void OnResumeButton()
    {
        _isPaused = false;
        _character.InputActions.Enable();
        if (true) _character.FirstCollision = true;
        ResumeGame();
        playingUI.SetActive(true);
        pauseUI.SetActive(false);
        _blur.enabled = false;
        stopwatch.TimeElapsed = _time;
    }

    public void OnCheckpointButton()
    {
        _character.ReturnToCheckpoint();
        OnResumeButton();
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}