using Input;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    // ReSharper disable once MemberCanBePrivate.Global
    public static GameUIManager Instance { get; private set; }
    private CharacterInput _inputActions;
    public AudioMixer audioMixer;
    public Camera mainCamera;
    private Blur _blur;
    public GameObject pauseUI;
    public GameObject playingUI;
    public bool isPaused {get; private set;}

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
    }

    private void OnEnable()
    {
        _inputActions.Player.Pause.performed += _ =>
        {
            isPaused = true;
            playingUI.SetActive(false);
            pauseUI.SetActive(true);
        };
    }

    public void OnBackToMainMenu() //TODO: delete music manager and GameUIManager
    {
        SceneManager.LoadScene(0);
    }

    public void OnResumeButton()
    {
        
    }
}
