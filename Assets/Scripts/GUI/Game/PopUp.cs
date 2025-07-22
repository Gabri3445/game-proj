using TMPro;
using UnityEngine;

public class PopUp : MonoBehaviour
{
    public TMP_InputField nameInput;
    private GameInstance _instance;

    private void Awake()
    {
        _instance = GameObject.Find("GameInstanceObject").GetComponent<GameInstance>();
    }

    public void OnSubmit()
    {
        //TODO input check
        //TODO Check if playerId has already been entered and update the standing
        var playerName = nameInput.text;
        var points = _instance.points;
        var level = _instance.GetLevelNumber();
        _instance.saveSlot.leaderboard.Add(new LeaderboardStruct(playerName, level, points));
        _instance.SaveGame();
        OnClose();
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