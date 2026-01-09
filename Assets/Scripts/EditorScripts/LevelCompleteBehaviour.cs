using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//This script handles the logic that occurs when a level is completed.
public class LevelCompleteBehaviour : MonoBehaviour
{
    public SaveLoadManager saveLoadManager;
    public GridManager gridManager;

// Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

//called when the player successfully completes a level
    public void ShowLevelComplete()
    {
// Get the currently active scene
        Scene currentScene = SceneManager.GetActiveScene();

// Gets index of the level that has just been completed
    int currentLevelIndex = PlayerPrefs.GetInt("CurrentLevelID", 1);

// Unlocks next level if the player is completing levels in order
        if (ProgressManager.HighestUnlockedLevel == currentLevelIndex)
        {
            ProgressManager.HighestUnlockedLevel++;
        }

// If the player is in the PlayLevel scene, show the completion UI and pause the game
        if (currentScene.name == "PlayLevel")
        {
            gameObject.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
// respawn the player and mark the level as completed
            gridManager.RespawnPlayer();
            gridManager.setLevelComplete(true);
        }
    }

//Hides the level complete UI.
    public void HideLevelComplete()
    {
        gameObject.SetActive(false);
    }
}