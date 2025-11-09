using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLevelUI : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void OpenSaveUI() {
        gameObject.SetActive(true);
    }

    public void CloseSaveUI() {
        gameObject.SetActive(false);
    }
}