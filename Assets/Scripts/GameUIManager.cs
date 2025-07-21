using Input;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    #region GameOverFields

    public GameObject gameOverUI;

    #endregion

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

    #region PlayMenu

    public void OnCheckPointChange(int checkpointNumber)
    {
        checkpointText.text = $"Checkpoint {checkpointNumber}/{_gameInstance.TotalCheckpointNumber}";
    }

    #endregion

    #region PauseMenuFields

    private CharacterInput _inputActions;
    private MusicManager _musicManager;
    public Camera mainCamera;
    private Blur _blur;
    public GameObject pauseUI;
    public Stopwatch stopwatch;
    public GameObject player;
    private Character _character;
    private Rigidbody _playerRb;
    private float _time;

    #endregion

    #region PlayMenuFields

    public GameObject playingUI;
    public TMP_Text checkpointText;
    private GameInstance _gameInstance;

    #endregion

    #region PauseMenu

    private void OnPause(InputAction.CallbackContext context)
    {
        _character.InputActions.Disable();
        _playerRb.Sleep();
        _playerRb.isKinematic = true;
        _musicManager.EnableHighpass();
        _time = stopwatch.TimeElapsed;
        IsPaused = true;
        playingUI.SetActive(false);
        pauseUI.SetActive(true);
        _blur.enabled = true; //TODO: could interpolate the blur too
    }

    public void OnBackToMainMenu()
    {
        _musicManager.DisableHighpassInstant();
        Destroy(_musicManager.gameObject);
        Destroy(gameObject);
        SceneManager.LoadScene(0);
    }

    public void OnResumeButton()
    {
        _character.InputActions.Enable();
        _playerRb.WakeUp();
        _playerRb.isKinematic = false;
        _musicManager.DisableHighpass();
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

    #endregion
}