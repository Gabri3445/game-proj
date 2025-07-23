using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using File = System.IO.File;
using Random = UnityEngine.Random;

public class GameInstance : MonoBehaviour
{
    public SaveSlot saveSlot = new(new List<LeaderboardStruct>());
    public bool isSaveGameLoaded;
    public Vector3 checkpoint;
    [CanBeNull] public GameUIManager gameUIManager;

    public int livesRemaining = 3;
    /// <summary>
    /// Current Level points
    /// </summary>
    public float points; 
    public float totalPoints;
    public readonly int LevelCount = 3;
    private string _savePath;
    public int TotalCheckpointNumber { get; private set; }

    // ReSharper disable once MemberCanBePrivate.Global
    public static GameInstance Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
#if UNITY_EDITOR
        TotalCheckpointNumber = GameObject.FindGameObjectsWithTag("Checkpoint").Length;
        try
        {
            gameUIManager = GameObject.Find("GameUIManagerObject").GetComponent<GameUIManager>();
        }
        catch (Exception)
        {
            // ignored
        }
#endif
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _savePath = Path.Combine(Application.persistentDataPath, "SaveSlot.json");
    }

    private void Start()
    {
        LoadSaveGame();
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

    public void SaveGame()
    {
        File.WriteAllText(_savePath, JsonUtility.ToJson(saveSlot, true));
    }

    private void MockLeaderboard()
    {
        for (var i = 0; i < 20; i++)
        {
            var id = "Player" + (i + 1);
            var level = Random.Range(0, 20);
            var randomPoints = Random.Range(0f, 1000f);

            var playerStruct = new LeaderboardStruct(id, level, randomPoints);
            saveSlot.leaderboard.Add(playerStruct);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        if (IsMainMenu()) return;
        livesRemaining = 3;
        TotalCheckpointNumber = GameObject.FindGameObjectsWithTag("Checkpoint").Length;
        try
        {
            gameUIManager = GameObject.Find("GameUIManagerObject").GetComponent<GameUIManager>();
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private bool IsMainMenu()
    {
        var sceneName = SceneManager.GetActiveScene().name;

        return sceneName == "MainMenu";
    }

    public int GetLevelNumber()
    {
        var level = 0;
        var sceneName = SceneManager.GetActiveScene().name;
        //if (sceneName == "FirstLevel") level = 1;

        level = sceneName switch
        {
            "FirstLevel" => 1,
            _ => level
        };

        return level;
    }

    private const float MaxScore = 1000;
    private static readonly TimeSpan MaxTime = TimeSpan.FromMinutes(2);
    private const float MinScore = 0;

    public float CalculatePoints(float time, bool addToScore = false)
    {
        var points = Mathf.Lerp(MaxScore, MinScore, (float)(time / MaxTime.TotalSeconds));
        if (addToScore)
            totalPoints += points;
        return points;
    }
}