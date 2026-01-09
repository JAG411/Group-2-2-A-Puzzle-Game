using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsUI : MonoBehaviour
{
    
    private bool isOpen = false;
    public CameraMovement mainCamera;
    public GameObject mainMenuButtons;
    public GameObject gameTitle;
    public GameObject levelSelectionPanel;


    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void OpenSettingsUI() {
        this.gameObject.SetActive(true);
        isOpen = true;

        // Hide main menu buttons
        if (mainMenuButtons != null)
            mainMenuButtons.SetActive(false);

        // Hide game title
        if (gameTitle != null)
            gameTitle.SetActive(false);

        if (mainCamera != null)
            mainCamera.StopMovement();
    }

    public void CloseSettingsUI() {
        this.gameObject.SetActive(false);
        isOpen = false;

        // Show main menu buttons
        if (mainMenuButtons != null)
            mainMenuButtons.SetActive(true);
        
        // Show game title
        if (gameTitle != null)
            gameTitle.SetActive(true);

        if (mainCamera != null)
            mainCamera.StartMovement();
    }

    public void ToggleSettingsUI() {
        if (isOpen) {
            CloseSettingsUI();
        } else {
            OpenSettingsUI();
        }
    }

    public void GoToLevelSelect()
    {
        SceneManager.LoadScene("LevelSelection");
    }
}