using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabOptionsMenu : MonoBehaviour
{

    private GameObject targetPrefab;

    // Start is called before the first frame update
    void Start()
    {
        targetPrefab = null;
        gameObject.SetActive(false);
    }

    public void OpenMenu(GameObject prefab) {
        targetPrefab = prefab;
        transform.position = Input.mousePosition;
        gameObject.SetActive(true);
    }

    public void CloseMenu() {
        targetPrefab = null;
        gameObject.SetActive(false);
    }

    public void DeletePrefab() {
        Destroy(targetPrefab);
        CloseMenu();
    }

    public void RotatePrefab() {
        targetPrefab.transform.Rotate(0, 90f, 0);
    }
}
