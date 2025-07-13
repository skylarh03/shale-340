using System;
using UnityEngine;

[Serializable]
public struct ScoreInfo : IComparable<ScoreInfo>
{
    public string PlayerName;
    public int Score;

    public ScoreInfo(string playerName, int score)
    {
        this.PlayerName = playerName;
        this.Score = score;
    }

    // for sorting by score (ascending)
    public int CompareTo(ScoreInfo other)
    {
        // if scores are equal, sort alphabetically by name
        int result = this.Score.CompareTo(other.Score);
        if (result == 0)
        {
            result = this.PlayerName.CompareTo(other.PlayerName);
            return result;
        }
        return (result * -1); // only happens if not sorting by name
    }

    public void PrintScore()
    {
        Debug.LogFormat("{0} - {1}", PlayerName, Score);
    }
}
