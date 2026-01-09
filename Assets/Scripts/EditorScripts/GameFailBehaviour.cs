using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFailBehaviour : MonoBehaviour
{
    void Start()
    {
// Hide fail panel at the beginning of the level
        gameObject.SetActive(false);
    }
// Shows fail screen
    public void ShowFail()
    {
// Activate fail panel
        gameObject.SetActive(true);

// Pause the game
        Time.timeScale = 0f;
    }
// Reloads the current level
    public void RetryLevel()
    {
// Resume time before reloading
        Time.timeScale = 1f;

// Reload the active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}