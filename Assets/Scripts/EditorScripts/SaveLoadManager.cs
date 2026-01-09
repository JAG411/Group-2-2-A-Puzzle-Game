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
    public int groundThemeIndex;
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
    public GroundTheme groundTheme;

    public Transform levelPanel;
    public Transform levelSelectionUI;

    public TMP_InputField levelNameInput;
    public CameraMovement mainCameraMovement;

    public OverwriteLevelUI overwriteLevelUI;
    public SaveLevelUI saveLevelUI;

    [Header("Level Settings")]
    public string saveFileName = "tempLevelData";
    private string levelFolder;
    private string loadedLevelName = null;

    void Start()
    {
        Scene  currentScene=SceneManager.GetActiveScene();
        if (currentScene.name=="LevelEditor" && gridManager != null) {
                    gridManager.UIToggle();
        }
        levelFolder = Application.dataPath + "/CreatedLevels/";
        if (!Directory.Exists(levelFolder)) {
            Directory.CreateDirectory(levelFolder);
        }
        if (currentScene.name == "PlayLevel") {
            TextAsset[] levels = Resources.LoadAll<TextAsset>("Levels");
            for(int i = 0; i < levels.Length; i++)
            {
                int levelID = i + 1;
                TextAsset level = levels[i];

                Debug.Log("Found level in Resources: " + level.name);
                GameObject prefab = Array.Find(placeablePrefabs, p => p.name == "Level_Button");
                GameObject btnObj = Instantiate(prefab, levelPanel);
                Button btn = btnObj.GetComponent<Button>();

                btn.GetComponentInChildren<TMP_Text>().text = "Level " + levelID;

                btn.onClick.AddListener(() => LoadLevelFromResources(level.name));
            }
        } 
        else if (currentScene.name == "LevelEditor") {
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

    public void LevelAlreadyExists() {
        levelFolder = Application.dataPath + "/CreatedLevels/";
        if (!Directory.Exists(levelFolder)) {
            Directory.CreateDirectory(levelFolder);
        }

        if (File.Exists(levelFolder + saveFileName + ".json")) {
            Debug.Log("Level already exists: " + saveFileName);
            overwriteLevelUI.OpenOverwriteUI();
        } else {
            SaveLevel();
        }
    }

    public void SaveLevel() {
        LevelData levelData = new LevelData();
        levelData.levelName = saveFileName;

        levelData.playerStartPosition = gridManager.getPlayerSpawnPosition();
        levelData.playerSpawnRotation = gridManager.getPlayerSpawnRotation();
        levelData.grounPosition = gridManager.getGroundPosition();
        levelData.groundScale = gridManager.getGroundScale();
        levelData.groundThemeIndex = groundTheme.currentThemeIndex;
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
        File.WriteAllText(levelFolder + saveFileName + ".json", json);

        overwriteLevelUI.CloseOverwriteUI();
        saveLevelUI.CloseSaveUI();
        if (gridManager.isUIOpen())
        {
            gridManager.UIToggle();
        }
    }

    public void LoadLevel(string levelName) {
        loadedLevelName = levelName;
        saveFileName = levelName;
        levelNameInput.text = levelName;
        string filePath = levelFolder + "/" + levelName + ".json";
        if (!File.Exists(filePath)) return;

        string json = File.ReadAllText(filePath);
        LevelData levelData = JsonUtility.FromJson<LevelData>(json);
        LoadLevelFromData(levelData);
    }

    public void LoadLevelFromResources(string levelName) {
        TextAsset levelFile = Resources.Load<TextAsset>("Levels/" + levelName);
        if (levelFile == null) return;

        LevelData levelData = JsonUtility.FromJson<LevelData>(levelFile.text);
        LoadLevelFromData(levelData);
    }

    private void LoadLevelFromData(LevelData levelData) {
        foreach (Transform child in placementContainer) {
            Destroy(child.gameObject);
        }

        GameObject ground = GameObject.Find("Ground");
        ground.transform.position = levelData.grounPosition;
        ground.transform.localScale = levelData.groundScale;

        GroundTheme themeController=FindFirstObjectByType<GroundTheme>();
            if (themeController != null)
            {
                themeController.SetTheme(levelData.groundThemeIndex);
            }

        foreach (PlacedObjectData objData in levelData.placedObjects) {
            GameObject prefab = Array.Find(placeablePrefabs, p => p.name == objData.prefabName);
            if (prefab != null) {
                if (objData.prefabName == "Character") {
                    Instantiate(prefab, levelData.playerStartPosition, levelData.playerSpawnRotation, placementContainer);
                } else {
                    Instantiate(prefab, objData.position, objData.rotation, placementContainer);
                }
            }
        }
        HideLevelSelectionUI();
    }

    public void HideLevelSelectionUI() {
        levelSelectionUI.gameObject.SetActive(false);
        if (gridManager!=null){
            gridManager.UIToggle();
        }
        mainCameraMovement.StartMovement();
    }

    public void updateLevelName() {
        saveFileName = levelNameInput.text;
    }

    public bool IsEditingLoadedLevel()
    {
        return !string.IsNullOrEmpty(loadedLevelName);
    }

    public void OpenOverwriteFromEdit()
    {
        overwriteLevelUI.OpenOverwriteUI();
    }

    public void OpenSaveAsNew()
    {
        loadedLevelName = null;
        saveFileName = "";
        levelNameInput.text = "";

        saveLevelUI.gameObject.SetActive(true);
        gridManager.UIToggle();
    }
}