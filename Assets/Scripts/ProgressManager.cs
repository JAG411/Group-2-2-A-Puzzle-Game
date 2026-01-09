using UnityEngine;

public static class ProgressManager
{
    private const string LEVEL_KEY = "HighestUnlockedLevel";
    public static int TotalMainLevels = 7;

//Max level the player reached
    public static int HighestUnlockedLevel
    {
        get
        {
            if (!PlayerPrefs.HasKey(LEVEL_KEY))
            {
                PlayerPrefs.SetInt(LEVEL_KEY, 1);
            }

            return PlayerPrefs.GetInt(LEVEL_KEY);
        }
        set
        {
            PlayerPrefs.SetInt(LEVEL_KEY, value);
            PlayerPrefs.Save();
        }
    }
//called when a level is completed
    public static void CompleteLevel(int currentLevelIndex)
    {
        if (HighestUnlockedLevel == currentLevelIndex)
        {
            HighestUnlockedLevel++;
        }
    }

//checks if all levels are completed
    public static bool AllLevelsCompleted()
    {
        return HighestUnlockedLevel > TotalMainLevels;
    }

    // Debug
    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(LEVEL_KEY);
    }
}
