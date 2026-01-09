/*
Controls which buttons are visible in Content_Holder
based on selected main tab and environment sub-tabs for content panel
*/
using UnityEngine;

public class TabManager : MonoBehaviour
{
    public GameObject contentPanel;
    public GameObject contentHolder;
    public GameObject environmentSubTabPanel;

    ButtonInfo[] allButtons;
    MainTab? currentOpenTab = null;

    void Awake() // Error handling
    {
        if (contentHolder == null)
        {
            Debug.LogError("Content Holder is not assigned.");
            return;
        }

        allButtons = contentHolder.GetComponentsInChildren<ButtonInfo>(true);
    }

    void Start() // Initial UI state
    {
        currentOpenTab = null;

        if (contentPanel != null)
            contentPanel.SetActive(false);

        if (environmentSubTabPanel != null)
            environmentSubTabPanel.SetActive(false);

        HideAllButtons();
    }

    // Opens specific tabs and shows related prefab
    public void OpenEnvironmentTab()
    {
        if (currentOpenTab == MainTab.Environment)
        {
            CloseContentPanel();
            return;
        }

        currentOpenTab = MainTab.Environment;

        contentPanel.SetActive(true);
        HideAllButtons();
        environmentSubTabPanel.SetActive(true);
    }

    public void OpenCharactersTab()
    {
        ShowMainTab(MainTab.Characters);
    }

    public void OpenTriggersTab()
    {
        ShowMainTab(MainTab.Triggers);
    }

    public void OpenGoalsTab()
    {
        ShowMainTab(MainTab.Goals);
    }

    public void OpenUtilityTab()
    {
        ShowMainTab(MainTab.Utility);
    }

    // Environment tab specific method to show sub-tabs
    public void OpenEnvironmentGroup(EnvironmentGroup subTab)
    {
        foreach (var button in allButtons)
        {
            bool match =
                button.mainTab == MainTab.Environment &&
                button.environmentSubTab == subTab;

            button.gameObject.SetActive(match);
        }
    }

    // Environment sub-tabs
    public void OpenEnvironmentTrees()
    {
        OpenEnvironmentGroup(EnvironmentGroup.Trees);
    }

    public void OpenEnvironmentRocks()
    {
        OpenEnvironmentGroup(EnvironmentGroup.Rocks);
    }

    public void OpenEnvironmentGround()
    {
        OpenEnvironmentGroup(EnvironmentGroup.Ground);
    }

    public void OpenEnvironmentCrystals()
    {
        OpenEnvironmentGroup(EnvironmentGroup.Crystals);
    }

    void ShowMainTab(MainTab tab)
    {
        if (currentOpenTab == tab)
        {
            CloseContentPanel();
            return;
        }

        currentOpenTab = tab;

        contentPanel.SetActive(true);
        environmentSubTabPanel.SetActive(false);

        foreach (var button in allButtons)
            button.gameObject.SetActive(button.mainTab == tab);
    }

    void HideAllButtons()
    {
        foreach (var button in allButtons)
            button.gameObject.SetActive(false);
    }

    void CloseContentPanel()
    {
        currentOpenTab = null;

        HideAllButtons();
        environmentSubTabPanel.SetActive(false);
        contentPanel.SetActive(false);
    }
}