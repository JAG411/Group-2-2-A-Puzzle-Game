using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFailBehaviour : MonoBehaviour
{
    public GameObject fail;
    public void ShowFail()
    {
        Time.timeScale = 0f;
        fail.SetActive(true);
    }
    public void RetryLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}