using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLevelUI : MonoBehaviour
{

    public GridManager gridManager;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void OpenSaveUI()
    {
        if (gridManager.isUIOpen())
            return;
        if (gridManager.saveLoadManager != null &&
            gridManager.saveLoadManager.IsEditingLoadedLevel())
        {
            gridManager.saveLoadManager.OpenOverwriteFromEdit();
            return;
        }
        gameObject.SetActive(true);
        gridManager.UIToggle();
    }

    public void CloseSaveUI() {
        gameObject.SetActive(false);
        gridManager.UIToggle();
    }
}