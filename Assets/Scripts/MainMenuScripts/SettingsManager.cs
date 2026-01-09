using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{

    public SettingsUI settingsUI;
    public GridManager gridManager;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            settingsUI.ToggleSettingsUI();
            if (gridManager != null) {
                gridManager.UIToggle();
            }
        }
    }
}
