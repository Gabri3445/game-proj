using System.Collections.Generic;
using System.IO;
using UnityEngine;
using File = System.IO.File;

public class GameInstance : MonoBehaviour
{
    public SaveSlot saveSlot = new(new List<LeaderboardStruct>());
    public bool isSaveGameLoaded;
    private string _savePath;

    // ReSharper disable once MemberCanBePrivate.Global
    public static GameInstance Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _savePath = Path.Combine(Application.persistentDataPath, "SaveSlot.json");
    }

    private void Start()
    {
        LoadSaveGame();
    }


    public GameInstance GetGameInstance()
    {
        return Instance;
    }

    private void SetGameInstance(GameInstance gameInstance)
    {
        Instance = gameInstance;
    }

    private void LoadSaveGame()
    {
        while (true)
        {
            saveSlot.leaderboard.Clear();
            if (File.Exists(_savePath))
            {
                var json = File.ReadAllText(_savePath);
                saveSlot = JsonUtility.FromJson<SaveSlot>(json);
                Debug.Log("SaveSlot loaded from " + _savePath);
#if UNITY_EDITOR
                MockLeaderboard();
#endif
                isSaveGameLoaded = true;
            }
            else
            {
                Debug.LogWarning("Save not found in " + _savePath);
                File.WriteAllText(_savePath, JsonUtility.ToJson(saveSlot, true));
                Debug.Log("Save created in " + _savePath);
                continue;
            }

            break;
        }
    }

    private void MockLeaderboard()
    {
        for (var i = 0; i < 20; i++)
        {
            var id = "Player" + (i + 1);
            var level = Random.Range(0, 20);
            var points = Random.Range(0f, 1000f);

            var playerStruct = new LeaderboardStruct(id, level, points);
            saveSlot.leaderboard.Add(playerStruct);
        }
    }
}