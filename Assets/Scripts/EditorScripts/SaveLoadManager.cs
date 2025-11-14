using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[Serializable]
public class PlacedObjectData {
    public string prefabName;
    public Vector3 position;
    public Quaternion rotation;

    public PlacedObjectData(string name, Vector3 pos, Quaternion rot) {
        prefabName = name;
        position = pos;
        rotation = rot;
    }
}


[Serializable]
public class LevelData {
    public string levelName;
    public List<PlacedObjectData> placedObjects = new List<PlacedObjectData>();
    public Vector3 playerStartPosition;
    public Quaternion playerSpawnRotation;
    public Vector3 grounPosition;
    public Vector3 groundScale;
    public Vector3 northArrowsPosition;
    public Vector3 southArrowsPosition;
    public Vector3 eastArrowsPosition;
    public Vector3 westArrowsPosition;
}

public class SaveLoadManager : MonoBehaviour
{
    public Transform placementContainer;
    public GameObject[] placeablePrefabs;
    public GridManager gridManager;

    public Transform levelPanel;
    public Transform levelSelectionUI;

    public TMP_InputField levelNameInput;
    public CameraMovement mainCameraMovement;

    [Header("Level Settings")]
    // public string folderName = "Levels";
    public string saveFileName = "tempLevelData";
    private string levelFolder;

    // Start is called before the first frame update
    void Start()
    {
        levelFolder = Application.dataPath + "/CreatedLevels/";
        if (!Directory.Exists(levelFolder)) {
            Directory.CreateDirectory(levelFolder);
        }  

        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "PlayLevel") { 
            TextAsset[] levels = Resources.LoadAll<TextAsset>("Levels");
            foreach (TextAsset level in levels) {
                Debug.Log("Found level in Resources: " + level.name);
                GameObject prefab = Array.Find(placeablePrefabs, p => p.name == "Level_Button");
                GameObject btnObj = Instantiate(prefab, levelPanel);
                Button btn = btnObj.GetComponent<Button>();
                btn.GetComponentInChildren<TMP_Text>().text = level.name;
                btn.onClick.AddListener(() => LoadLevelFromResources(level.name));
            }
        } else if (currentScene.name == "LevelEditor") {
            string[] levels = Directory.GetFiles(levelFolder, "*.json");
            foreach (string level in levels) {
                string levelName = Path.GetFileNameWithoutExtension(level);
                Debug.Log("Found level in CreatedLevels: " + levelName);
                GameObject prefab = Array.Find(placeablePrefabs, p => p.name == "Level_Button");
                GameObject btnObj = Instantiate(prefab, levelPanel);
                Button btn = btnObj.GetComponent<Button>();
                btn.GetComponentInChildren<TMP_Text>().text = levelName;
                btn.onClick.AddListener(() => LoadLevel(levelName));
            }
        }
    }

    public void SaveLevel() {
        LevelData levelData = new LevelData();
        levelData.levelName = saveFileName;

        levelData.playerStartPosition = gridManager.getPlayerSpawnPosition();
        levelData.playerSpawnRotation = gridManager.getPlayerSpawnRotation();
        levelData.grounPosition = gridManager.getGroundPosition();
        levelData.groundScale = gridManager.getGroundScale();
        levelData.northArrowsPosition = gridManager.getNorthArrowsPosition();
        levelData.southArrowsPosition = gridManager.getSouthArrowsPosition();
        levelData.eastArrowsPosition = gridManager.getEastArrowsPosition();
        levelData.westArrowsPosition = gridManager.getWestArrowsPosition();

        foreach (Transform child in placementContainer)
        {
            PlacedObjectData objData = new PlacedObjectData(
                child.gameObject.name.Replace("(Clone)", "").Trim(),
                child.position,
                child.rotation
            );
            levelData.placedObjects.Add(objData);
        }

        string json = JsonUtility.ToJson(levelData, true);
        levelFolder = Application.dataPath + "/CreatedLevels/";
        if (!Directory.Exists(levelFolder)) {
            Directory.CreateDirectory(levelFolder);
        }  
        File.WriteAllText(levelFolder + saveFileName + ".json", json);
        Debug.Log("Level saved: " + levelFolder + saveFileName + ".json");
    }
    
    public void LoadLevel(String levelName) {
        Debug.Log("Loading level: " + levelName + " from " + levelFolder);
        string filePath = levelFolder + "/" + levelName + ".json";
        
        if (File.Exists(filePath)) {
            string json = File.ReadAllText(filePath);
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);

            // Clear existing objects
            foreach (Transform child in placementContainer) {
                Destroy(child.gameObject);
            }

            GameObject ground = GameObject.Find("Ground");
            ground.transform.position = levelData.grounPosition;
            ground.transform.localScale = levelData.groundScale;

            GameObject northArrows = GameObject.Find("Arrows_North");
            northArrows.transform.position = levelData.northArrowsPosition;
            GameObject southArrows = GameObject.Find("Arrows_South");
            southArrows.transform.position = levelData.southArrowsPosition;
            GameObject eastArrows = GameObject.Find("Arrows_East");
            eastArrows.transform.position = levelData.eastArrowsPosition;
            GameObject westArrows = GameObject.Find("Arrows_West");
            westArrows.transform.position = levelData.westArrowsPosition;

            // Instantiate saved objects
            foreach (PlacedObjectData objData in levelData.placedObjects) {
                GameObject prefab = Array.Find(placeablePrefabs, p => p.name == objData.prefabName);
                if (prefab != null) {
                    if (objData.prefabName == "Character") {
                        GameObject player = Instantiate(prefab, levelData.playerStartPosition, levelData.playerSpawnRotation, placementContainer);
                    } else {
                        GameObject obj = Instantiate(prefab, objData.position, objData.rotation, placementContainer);
                    }
                } else {
                    Debug.LogWarning("Prefab not found: " + objData.prefabName);
                }
            }

            Debug.Log("Level loaded: " + filePath);
            HideLevelSelectionUI();
        } else {
            Debug.LogError("Save file not found: " + filePath);
        }
    }

    public void LoadLevelFromResources(String levelName) {
        // string levelName = saveFileName;
        TextAsset levelFile = Resources.Load<TextAsset>("Levels/" + levelName);
        
        if (levelFile != null) {
            string json = levelFile.text;
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);
            
            // Load the level data as before...
            LoadLevelFromData(levelData);
        } else {
            Debug.LogError("Level file not found in Resources: " + levelName);
        }
    }

    private void LoadLevelFromData(LevelData levelData) {
        // Clear existing objects
        foreach (Transform child in placementContainer) {
            Destroy(child.gameObject);
        }

        GameObject ground = GameObject.Find("Ground");
        ground.transform.position = levelData.grounPosition;
        ground.transform.localScale = levelData.groundScale;

        // Instantiate saved objects
        foreach (PlacedObjectData objData in levelData.placedObjects) {
            GameObject prefab = Array.Find(placeablePrefabs, p => p.name == objData.prefabName);
            if (prefab != null) {
                if (objData.prefabName == "Character") {
                    GameObject player = Instantiate(prefab, levelData.playerStartPosition, levelData.playerSpawnRotation, placementContainer);
                } else {
                    GameObject obj = Instantiate(prefab, objData.position, objData.rotation, placementContainer);
                }
            }
        }
        HideLevelSelectionUI();
    }

    public void HideLevelSelectionUI() {
        levelSelectionUI.gameObject.SetActive(false);
        mainCameraMovement.StartMovement();
    }

    public void updateLevelName() {
        saveFileName = levelNameInput.text;
    }

}
