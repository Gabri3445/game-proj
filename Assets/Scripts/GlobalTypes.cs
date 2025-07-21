using System;
using System.Collections.Generic;

[Serializable]
public struct LeaderboardStruct
{
    public string playerId;
    public int level;
    public float points;

    public LeaderboardStruct(string playerId, int level, float points)
    {
        this.playerId = playerId;
        this.level = level;
        this.points = points;
    }

    public override string ToString()
    {
        return $"ID: {playerId}, Level: {level}, Points: {points}";
    }
}

[Serializable]
public class SaveSlot
{
    public List<LeaderboardStruct> leaderboard;

    public SaveSlot(List<LeaderboardStruct> leaderboard)
    {
        this.leaderboard = leaderboard;
    }
}

public enum CharacterPosition
{
    Left,
    Center,
    Right
}