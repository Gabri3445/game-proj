using System;
using Input;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    // ReSharper disable once MemberCanBePrivate.Global
    public static GameUIManager Instance { get; private set; }
    private CharacterInput _inputActions;
    private MusicManager  _musicManager;
    public Camera mainCamera;
    private Blur _blur;
    public GameObject pauseUI;
    public GameObject playingUI;
    public Stopwatch stopwatch;
    private float _time;
    public bool IsPaused {get; private set;}

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
        _inputActions = new CharacterInput();
        _inputActions.Enable();
        _musicManager = GameObject.Find("MusicManagerObject").GetComponent<MusicManager>();
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

    private void OnPause(InputAction.CallbackContext context)
    {
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
        _musicManager.DisableHighpass();
        playingUI.SetActive(true);
        pauseUI.SetActive(false);
        _blur.enabled = false;
        stopwatch.TimeElapsed = _time;
    }
}
