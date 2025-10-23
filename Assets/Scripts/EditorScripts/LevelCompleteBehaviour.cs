using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteBehaviour : MonoBehaviour
{

    public SaveLoadManager saveLoadManager;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void ShowLevelComplete() {
        gameObject.SetActive(true);
        Time.timeScale = 0f; // Pause the game 
        saveLoadManager.SaveLevel();
    }
}
