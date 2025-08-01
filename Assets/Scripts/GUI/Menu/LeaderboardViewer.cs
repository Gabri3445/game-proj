using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class LeaderboardViewer : MonoBehaviour
{
    public GameObject slotPrefab;
    private GameInstance _instance;

    private void Start()
    {
        _instance = GameInstance.Instance;
        StartCoroutine(WaitForSaveGame());
    }

    private IEnumerator WaitForSaveGame()
    {
        while (!_instance.isSaveGameLoaded) yield return null;

        _instance.saveSlot.leaderboard = _instance.saveSlot.leaderboard.OrderByDescending(x => x.level)
            .ThenByDescending(x => x.points).ToList();
        for (var i = 0; i < _instance.saveSlot.leaderboard.Count; i++)
        {
            var place = _instance.saveSlot.leaderboard[i];
            var slot = Instantiate(slotPrefab, transform).GetComponent<LeaderboardSlot>();
            if (i == 0) slot.crown.gameObject.SetActive(true);
            slot.playerId.text = $"{place.playerId}";
            slot.level.text = $"{place.level}";
            slot.points.text = $"{Math.Round(place.points, 2)}";
        }
    }
}