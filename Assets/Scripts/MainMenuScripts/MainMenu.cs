using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Loads the level editor scene
    public void PlayGame()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadLevelEditor()
    {
        SceneManager.LoadScene(1);
    }

    // Quits the game application
    public void QuitGame()
    {
        //Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
