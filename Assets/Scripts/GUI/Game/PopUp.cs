using TMPro;
using UnityEngine;

public class PopUp : MonoBehaviour
{
    public TMP_InputField nameInput;
    private GameInstance _instance;

    private void Start()
    {
        _instance = GameInstance.Instance;
    }

    public void OnSubmit()
    {
        var playerName = nameInput.text;
        if (playerName.Length is < 1 or > 8)
            //display error
            return;

        var points = _instance.points;
        var level = _instance.GetLevelNumber();
        if (_instance.saveSlot.leaderboard.Exists(x => x.playerId == playerName))
        {
            var earlierRecord = _instance.saveSlot.leaderboard.Find(x => x.playerId == playerName);
            if (!(points > earlierRecord.points) && level <= earlierRecord.level) return;

            var index = _instance.saveSlot.leaderboard.FindIndex(x => x.playerId == playerName);
            _instance.saveSlot.leaderboard.RemoveAt(index);
            _instance.saveSlot.leaderboard.Add(new LeaderboardStruct(playerName, level, points));


            return;
        }

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