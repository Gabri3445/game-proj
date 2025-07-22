using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    public TMP_InputField nameInput;
    public Button submitButton;
    public Button closeButton;
    private GameInstance _instance;

    private void Awake()
    {
        _instance = GameObject.Find("GameInstanceObject").GetComponent<GameInstance>();
    }

    public void OnSubmit()
    {
        //TODO input check
        var playerName = nameInput.text;
        var points = _instance.points;
        var level = _instance.GetLevelNumber();
        _instance.saveSlot.leaderboard.Add(new LeaderboardStruct(playerName, level, points));
        _instance.SaveGame();
        if (_instance.gameUIManager != null) _instance.gameUIManager.OnBackToMainMenu();
    }

    public void OnClose()
    {
        nameInput.text = "";
        gameObject.SetActive(false);
    }

    public void Open()
    {
        nameInput.text = "";
        gameObject.SetActive(true);
    }
}