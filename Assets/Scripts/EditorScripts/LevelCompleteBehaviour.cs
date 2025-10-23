using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteBehaviour : MonoBehaviour
{

    public SaveLoadManager saveLoadManager;
    public GridManager gridManager;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void ShowLevelComplete() {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "PlayLevel") {
            gameObject.SetActive(true);
            Time.timeScale = 0f;
        } else {
            gridManager.RespawnPlayer();
            gridManager.setLevelComplete(true);
        }
    }

    public void HideLevelComplete() {
        gameObject.SetActive(false);
    }
}
