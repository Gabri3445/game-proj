using Input;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public GameObject playingUI;
    public TMP_Text playingCheckpointText;
    public TMP_Text playingLivesText;

    public GameObject gameOverUI;
    public TMP_Text gameOverLevelText;
    public TMP_Text gameOverTimerText;
    public PopUp gameOverNamePopup;
    public TMP_Text gameOverLivesLeftText;

    public Camera mainCamera;
    public GameObject pauseUI;
    public Stopwatch stopwatch;
    public GameObject player;

    public GameObject levelEndUI;
    public TMP_Text levelEndLevelText;
    public TMP_Text levelEndPointsText;
    public TMP_Text levelEndTimerText;
    public Button levelEndContinueButton;
    public PopUp levelEndNamePopup;
    public TMP_Text levelEndLevelsLeftText;


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
        _gameInstance = GameInstance.Instance;
        _musicManager = MusicManager.Instance;
        playingCheckpointText.text = $"Checkpoint {0}/{_gameInstance.TotalCheckpointNumber}";
        playingLivesText.text = $"Lives remaining: {_gameInstance.livesRemaining}";
    }

    private void OnEnable()
    {
        _inputActions.Player.Pause.performed += OnPause;
    }

    private void OnDestroy()
    {
        _inputActions.Player.Pause.performed -= OnPause;
        _inputActions.Disable();
    }

    private void EnablePlayingUI()
    {
        pauseUI.SetActive(false);
        gameOverUI.SetActive(false);
        playingUI.SetActive(true);
        levelEndUI.SetActive(false);
    }

    private void EnableLevelEndUI()
    {
        pauseUI.SetActive(false);
        gameOverUI.SetActive(false);
        playingUI.SetActive(false);
        levelEndUI.SetActive(true);
    }

    private void EnableGameOverUI()
    {
        pauseUI.SetActive(false);
        gameOverUI.SetActive(true);
        playingUI.SetActive(false);
        levelEndUI.SetActive(false);
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
            gameOverLivesLeftText.text =
                $"But you still have {_gameInstance.livesRemaining} {(_gameInstance.livesRemaining != 1 ? "lives" : "life")} left!";
        else
            gameOverLivesLeftText.gameObject.SetActive(false);
        gameOverTimerText.text = $"Time: {stopwatch.TimeToString(_time)}";
    }

    public void OnRetry()
    {
        if (_gameInstance.livesRemaining == 0)
        {
            _gameInstance.points = 0;
            _gameInstance.totalPoints = 0;
            _gameInstance.livesRemaining = 3;
            _musicManager.DisableHighpassInstant();
            _musicManager.DisableLowpassInstant();
            SceneManager.LoadScene("FirstLevel");
        }

        _gameInstance.totalPoints -= _gameInstance.points;
        _blur.enabled = false;
        _musicManager.DisableLowpass();
        gameOverUI.SetActive(false);
        OnCheckpointButton();
    }

    public void GameEndOnSaveLeaderboard()
    {
        gameOverNamePopup.Open();
    }


    public void OnCheckPointChange(int checkpointNumber)
    {
        playingCheckpointText.text = $"Checkpoint {checkpointNumber}/{_gameInstance.TotalCheckpointNumber}";
    }

    private void EnablePauseUI()
    {
        pauseUI.SetActive(true);
        gameOverUI.SetActive(false);
        playingUI.SetActive(false);
        levelEndUI.SetActive(false);
    }


    private void OnPause(InputAction.CallbackContext context)
    {
        if (gameOverUI.activeSelf || levelEndUI.activeSelf) return;

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

    public void OnLevelEnd()
    {
        _isPaused = true;
        EnableLevelEndUI();
        PauseGame();
        _musicManager.EnableLowpass();
        _blur.enabled = true;
        _time = stopwatch.TimeElapsed;
        levelEndLevelText.text = $"Level: {_gameInstance.GetLevelNumber()}";
        _gameInstance.points = _gameInstance.CalculatePoints(_time, true);
        levelEndPointsText.text = $"Points: {_gameInstance.totalPoints}";
        levelEndTimerText.text = $"Time: {stopwatch.TimeToString(_time)}";
        var levelsLeft = _gameInstance.LevelCount - _gameInstance.GetLevelNumber();
        _musicManager.PlayMicrowaveBeepSfx();
        if (_gameInstance.GetLevelNumber() < 3)
        {
            levelEndLevelsLeftText.text = $"But you still have {levelsLeft} level{(levelsLeft != 1 ? "s" : "")} left!";
            return;
        }


        //game ended
        levelEndContinueButton.gameObject.SetActive(false);
        levelEndLevelsLeftText.text = "And you've won!";
    }

    public void OnContinueButton()
    {
        var index = SceneManager.GetActiveScene().buildIndex + 1;
        if (index < SceneManager.sceneCountInBuildSettings)
        {
            _gameInstance.CalculatePoints(_time);
            _musicManager.DisableLowpassInstant();
            _musicManager.DisableHighpassInstant();
            _musicManager.PlayMicrowaveSfx();
            SceneManager.LoadScene(index);
        }
        else

            Debug.LogError("Non existent scene. Forgot the to add the scene to build scene list?");
    }

    public void LevelEndSaveLeaderboard()
    {
        levelEndNamePopup.Open();
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