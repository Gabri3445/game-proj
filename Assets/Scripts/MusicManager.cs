using UnityEngine;

public class MusicManager : MonoBehaviour
{
    //TODO: destroy the object when exiting to main menu
    // ReSharper disable once MemberCanBePrivate.Global
    public static MusicManager Instance {get; private set;}

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
