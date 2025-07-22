using Input;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    //TODO: Refactor all this in three separate classes
    public GameObject gameOverUI;
    public TMP_Text gameOverLevelText;
    public TMP_Text gameOverPointsText;
    public TMP_Text gameOverTimerText;
    public PopUp gameOverNamePopup;
    public Camera mainCamera;
    public GameObject pauseUI;
    public Stopwatch stopwatch;
    public GameObject player;
    public TMP_Text gameOverLivesLeftText;


    public GameObject playingUI;
    public TMP_Text playingCheckpointText;
    public TMP_Text playingLivesText;

    public GameObject levelEnd;
    private Blur _blur;
    private Character _character;
    private GameInstance _gameInstance;
    private CharacterInput _inputActions;
    private bool _isPaused;
    private MusicManager _musicManager;
    private float _time;

    private void Awake()
    {
        _blur = mainCamera.GetComponent<Blur>();
        EnablePlayingUI();
        _inputActions = new CharacterInput();
        _inputActions.Enable();
        _character = player.GetComponent<Character>();
    }


    private void Start()
    {
        playingCheckpointText.text = $"Checkpoint {0}/{_gameInstance.TotalCheckpointNumber}";
        playingLivesText.text = $"Lives remaining: {_gameInstance.livesRemaining}";
        _gameInstance = GameInstance.Instance;
        _musicManager = MusicManager.Instance;
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

    private void EnablePlayingUI()
    {
        pauseUI.SetActive(false);
        gameOverUI.SetActive(false);
        playingUI.SetActive(true);
        levelEnd.SetActive(false);
    }

    private void EnableLevelEndUI()
    {
        pauseUI.SetActive(false);
        gameOverUI.SetActive(false);
        playingUI.SetActive(false);
        levelEnd.SetActive(true);
    }

    private void EnableGameOverUI()
    {
        pauseUI.SetActive(false);
        gameOverUI.SetActive(true);
        playingUI.SetActive(false);
        levelEnd.SetActive(false);
    }

    public void OnGameOver()
    {
        playingLivesText.text = $"Lives remaining: {_gameInstance.livesRemaining}";
        _isPaused = true;
        _character.InputActions.Disable();
        PauseGame();
        _musicManager.EnableLowpass();
        _time = stopwatch.TimeElapsed;
        EnableGameOverUI();
        _blur.enabled = true;
        gameOverLevelText.text = $"Level: {_gameInstance.GetLevelNumber()}";
        if (_gameInstance.livesRemaining > 0)
            gameOverLivesLeftText.text = $"But you still have {_gameInstance.livesRemaining} lives left!";
        else
            gameOverLivesLeftText.gameObject.SetActive(false);
        gameOverTimerText.text = $"Time: {stopwatch.TimeToString(_time)}";
        gameOverPointsText.text = $"Points: {_gameInstance.points}";
    }

    public void OnRetry()
    {
        //TODO, check for lives and bring to checkpoint otherwise bring to level 1
        if (_gameInstance.livesRemaining == 0)
        {
            _musicManager.DisableHighpassInstant();
            _musicManager.DisableLowpassInstant();
            SceneManager.LoadScene("FirstLevel");
        }

        _blur.enabled = false;
        _musicManager.DisableLowpass();
        gameOverUI.SetActive(false);
        OnCheckpointButton();
    }

    public void OnSaveLeaderboard()
    {
        gameOverNamePopup.Open();
    }


    public void OnCheckPointChange(int checkpointNumber)
    {
        playingCheckpointText.text = $"Checkpoint {checkpointNumber}/{_gameInstance.TotalCheckpointNumber}";
    }

    private void EnablePauseUI()
    {
        pauseUI.SetActive(false);
        gameOverUI.SetActive(true);
        playingUI.SetActive(false);
        levelEnd.SetActive(false);
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
        _musicManager.EnableHighpass();
        _time = stopwatch.TimeElapsed;
        EnablePauseUI();
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
        _musicManager.DisableHighpass();
        EnablePlayingUI();
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