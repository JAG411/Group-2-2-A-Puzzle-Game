using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFailBehaviour : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void ShowFail()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RetryLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}