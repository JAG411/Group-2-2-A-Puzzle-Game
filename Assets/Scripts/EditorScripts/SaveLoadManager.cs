using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

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
}

public class SaveLoadManager : MonoBehaviour
{
    public Transform placementContainer;
    public GameObject[] placeablePrefabs;
    public GridManager gridManager;

    public string saveFileName = "tempLevelData";
    public string levelFolder;

    [Header("Level Settings")]
    public string folderName = "Levels";

    // Start is called before the first frame update
    void Start()
    {
        levelFolder = Application.dataPath + "/" + folderName + "/";
        
        if (!Directory.Exists(levelFolder)) {
            Directory.CreateDirectory(levelFolder);
        }
        LoadLevel();
    }

    public void SaveLevel() {
        LevelData levelData = new LevelData();
        levelData.levelName = saveFileName;

        levelData.playerStartPosition = gridManager.getPlayerSpawnPosition();
        levelData.playerSpawnRotation = gridManager.getPlayerSpawnRotation();

        foreach (Transform child in placementContainer)
        {
            PlacedObjectData objData = new PlacedObjectData(
                child.gameObject.name.Replace("(Clone)", "").Trim(),
                child.position,
                child.rotation
            );
            levelData.placedObjects.Add(objData);
        }
        
        levelFolder = Application.dataPath + "/" + folderName + "/";
        if (!Directory.Exists(levelFolder))
        {
            Directory.CreateDirectory(levelFolder);
        }

        string json = JsonUtility.ToJson(levelData, true);
        levelFolder = Application.dataPath + "/CreatedLevels/";
        if (!Directory.Exists(levelFolder)) {
            Directory.CreateDirectory(levelFolder);
        }  
        File.WriteAllText(levelFolder + saveFileName + ".json", json);
        Debug.Log("Level saved: " + levelFolder + saveFileName + ".json");
    }
    
    public void LoadLevel() {
        if (string.IsNullOrEmpty(levelFolder)) {
            levelFolder = Application.dataPath + "/Levels/";
            if (!Directory.Exists(levelFolder)) {
                Directory.CreateDirectory(levelFolder);
            }  
        }
        Debug.Log("Loading level: " + saveFileName + " from " + levelFolder);
        string filePath = levelFolder + saveFileName + ".json";
        
        if (File.Exists(filePath)) {
            string json = File.ReadAllText(filePath);
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);

            // Clear existing objects
            foreach (Transform child in placementContainer) {
                Destroy(child.gameObject);
            }

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
        } else {
            Debug.LogError("Save file not found: " + filePath);
        }
    }

    public void LoadLevelFromResources() {
        string levelName = saveFileName;
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
    }

}
