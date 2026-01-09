using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverwriteLevelUI : MonoBehaviour
{

    public GridManager gridManager;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void OpenOverwriteUI() {
        gameObject.SetActive(true);
    }

    public void CloseOverwriteUI() {
        gameObject.SetActive(false);
        gridManager.saveLoadManager.saveLevelUI.gameObject.SetActive(true);
    }

    public void Overwrite() {
        gridManager.UIToggle();
        CloseOverwriteUI();
    }
}