using System.Collections.Generic;
using UnityEngine;

public static class StatsManager
{
    private const string LAST_PLAYER_KEY = "LAST_PLAYER";
    private const string PROFILE_KEY_PREFIX = "PLAYER_STATS_";
    public const string PROFILE_NAME_PREFIX = "PLAYER_NAME_"; // Made public for access from ProfileMenu

    // ------------------ STATS ------------------
    public static void SaveStat(int profileIndex, string statName, int statValue)
    {
        StatCollection collection = LoadStatsInternal(profileIndex);

        StatEntry existing = collection.stats.Find(s => s.name == statName);

        if (existing != null)
            existing.score = statValue;
        else
            collection.stats.Add(new StatEntry { name = statName, score = statValue });

        string json = JsonUtility.ToJson(collection);
        PlayerPrefs.SetString(PROFILE_KEY_PREFIX + profileIndex, json);
        PlayerPrefs.Save();
    }

    public static StatCollection LoadStats(int profileIndex)
    {
        return LoadStatsInternal(profileIndex);
    }

    private static StatCollection LoadStatsInternal(int profileIndex)
    {
        string key = PROFILE_KEY_PREFIX + profileIndex;

        if (!PlayerPrefs.HasKey(key))
            return new StatCollection();

        string json = PlayerPrefs.GetString(key);
        return JsonUtility.FromJson<StatCollection>(json);
    }

    // ------------------ PROFILE NAME ------------------
    public static void SavePlayerName(int profileIndex, string name)
    {
        PlayerPrefs.SetString(PROFILE_NAME_PREFIX + profileIndex, name);
        PlayerPrefs.Save();
        SaveLastPlayer(profileIndex);
    }

    public static string LoadPlayerName(int profileIndex)
    {
        // Return the name only if it exists, otherwise return an empty string.
        return PlayerPrefs.GetString(PROFILE_NAME_PREFIX + profileIndex, "");
    }

    public static bool ProfileNameExists(int profileIndex)
    {
        return PlayerPrefs.HasKey(PROFILE_NAME_PREFIX + profileIndex);
    }

    // ------------------ LAST PLAYER ------------------
    public static void SaveLastPlayer(int index)
    {
        PlayerPrefs.SetInt(LAST_PLAYER_KEY, index);
        PlayerPrefs.Save();
    }

    public static int LoadLastPlayer()
    {
        return PlayerPrefs.GetInt(LAST_PLAYER_KEY, 0);
    }

    public static void ClearProfile(int profileIndex)
    {
        PlayerPrefs.DeleteKey("PLAYER_STATS_" + profileIndex);
        PlayerPrefs.DeleteKey("PLAYER_NAME_" + profileIndex);

        // Si este era el último jugador, resetear a 0
        int last = PlayerPrefs.GetInt("LAST_PLAYER", 0);
        if (last == profileIndex)
            PlayerPrefs.SetInt("LAST_PLAYER", 0);

        PlayerPrefs.Save();
    }

    public static void ClearAllProfiles()
    {
        for (int i = 0; i < 4; i++)
        {
            PlayerPrefs.DeleteKey("PLAYER_STATS_" + i);
            PlayerPrefs.DeleteKey("PLAYER_NAME_" + i);
        }

        PlayerPrefs.DeleteKey("LAST_PLAYER");
        PlayerPrefs.Save();
    }
}


[System.Serializable]
public class StatEntry
{
    public string name;
    public int score;
}

[System.Serializable]
public class StatCollection
{
    public List<StatEntry> stats = new List<StatEntry>();
}
