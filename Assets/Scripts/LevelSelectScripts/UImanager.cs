using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject levelLayer;

    public GameObject settingsPanel;
    public GameObject levelSelectionPanel;

    bool panelOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (panelOpen)
                CloseAllPanels();
            else
                OpenSettings();
        }
    }
//Open
    public void OpenSettings()
    {
        CloseAllPanels();

        panelOpen = true;
        levelLayer.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OpenLevelSelection()
    {
        CloseAllPanels();

        panelOpen = true;
        levelLayer.SetActive(false);
        levelSelectionPanel.SetActive(true);
    }
    //Close
    public void CloseAllPanels()
    {
        panelOpen = false;

        settingsPanel.SetActive(false);
        levelSelectionPanel.SetActive(false);
        levelLayer.SetActive(true);
    }
//Scene Manager
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToLevelSelect()
    {
        SceneManager.LoadScene("LevelSelection");
    }
}
