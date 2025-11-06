using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public Vector3 cellSize = Vector3.one;
    public Camera mainCamera;
    public bool placement = false;
    public GameObject prefabToPlace;
    public Transform placementContainer;
    private GameObject ghostObject;
    private float offsetY = 0.5f;
    private Quaternion rotation;
    public PrefabOptionsMenu prefabOptionsMenu;
    public bool UI = false;
    public SaveLoadManager saveLoadManager;
    private Vector3 playerStartPosition;
    private Quaternion playerStartRotation;
    public bool levelComplete = false;
    public SaveLevelUI saveLevelUI;

    public Vector3 GridToWorld(Vector3Int gridPos)
    {
        return Vector3.Scale(gridPos, cellSize);
    }

    public Vector3Int WorldToGrid(Vector3 worldPos)
    {
        return Vector3Int.RoundToInt(new Vector3(
            worldPos.x / cellSize.x,
            worldPos.y / cellSize.y,
            worldPos.z / cellSize.z
        ));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        for (int x = -10; x < 11; x++)
        {
            for (int z = -10; z < 11; z++)
            {
                Vector3 worldPos = GridToWorld(new Vector3Int(x, 0, z));
                Gizmos.DrawWireCube(worldPos, cellSize);
            }
        }
    }

    void Start() {
        // Scene currentScene = SceneManager.GetActiveScene();
        // if (currentScene.name == "PlayLevel") { 
        //     saveLoadManager.LoadLevelFromResources();
        // } else if (currentScene.name == "LevelEditor") {
        //     saveLoadManager.LoadLevelFromResources();
        // }
    }

     void Update()
    {
        if (placement) {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Input.GetKeyDown(KeyCode.R)) {
                rotation *= Quaternion.Euler(0, 90, 0);
                ghostObject.transform.rotation = rotation;
            }

            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                if (hit.collider.gameObject.tag != "Ground") {
                    updateGhostColor(Color.red);
                } else {
                    updateGhostColor(Color.green);
                }
                Vector3 worldPos = hit.point;
                Vector3Int gridPos = WorldToGrid(worldPos);
                Vector3 snappedWorldPos = GridToWorld(gridPos);
                PlacementOffset offsetComponent = prefabToPlace.GetComponent<PlacementOffset>();
                if (offsetComponent != null) {
                    offsetY = offsetComponent.offsetY;
                }
                snappedWorldPos.y = offsetY; // Adjust for object height

                ghostObject.transform.position = snappedWorldPos;
            }

            if (Input.GetMouseButtonDown(0)) {
                if (Physics.Raycast(ray, out hit)) {
                    if (hit.collider.gameObject.tag != "Ground") {
                        return;
                    }
                    Vector3 worldPos = hit.point;
                    Vector3Int gridPos = WorldToGrid(worldPos);
                    Vector3 snappedWorldPos = GridToWorld(gridPos);
                    PlacementOffset offsetComponent = prefabToPlace.GetComponent<PlacementOffset>();
                    if (offsetComponent != null) {
                        offsetY = offsetComponent.offsetY;
                    }
                    snappedWorldPos.y = offsetY; // Adjust for object height
                    if (prefabToPlace.tag == "Player") {
                        playerStartPosition = snappedWorldPos;
                        playerStartRotation = Quaternion.identity;
                    }
                    setLevelComplete(false);
                    Instantiate(prefabToPlace, snappedWorldPos, Quaternion.identity, placementContainer);
                }

            } else if (Input.GetMouseButtonDown(1)) {
                placement = false;
                prefabToPlace = null;
                if (ghostObject != null) {
                    Destroy(ghostObject);
                }
                ghostObject = null;
            }
        } else if (!UI && !placement) {
            if (Input.GetMouseButtonDown(0)) {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit)) {
                    Scene currentScene = SceneManager.GetActiveScene();
                    if (hit.collider.gameObject.tag == "Rotatable" || hit.collider.gameObject.tag == "Direction" || hit.collider.gameObject.tag == "Goal" || (hit.collider.gameObject.tag == "Player" && currentScene.name == "EditorScene")) {
                        UI = true;
                        prefabOptionsMenu.OpenMenu(hit.collider.gameObject);
                    }
                }
            }
        } else if (Input.GetMouseButtonDown(1)) {
            UI = false;
            prefabOptionsMenu.CloseMenu();
        }
    }

    public void setPrefabPlacement(Object prefab) {
        GameObject obj = (GameObject)prefab;
        prefabToPlace = obj;
        placement = true;
        CreateGhostObject();
        UI = false;
        prefabOptionsMenu.CloseMenu();
    }

    public void RotateButtonClick() {
        prefabOptionsMenu.RotatePrefab();
    }

    public void DeleteButtonClick() {
        prefabOptionsMenu.DeletePrefab();
        UI = false;
    }

    void CreateGhostObject() {
        if (ghostObject != null) {
            Destroy(ghostObject);
        }
        ghostObject = Instantiate(prefabToPlace);
        ghostObject.GetComponent<Collider>().enabled = false;

        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer render in renderers) {
            Material material = new Material(render.material);
            Color color = material.color;
            color.a = 0.5f;
            material.color = color;
            material.renderQueue = 3000; // Ensure it's rendered on top
            render.material = material;
        }

        Collider[] colliders = ghostObject.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders) {
            col.enabled = false;
        }

        Rigidbody[] rbs = ghostObject.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs) {
            rb.isKinematic = true;
            rb.detectCollisions = false;
            rb.useGravity = false;
        }

        MonoBehaviour[] scripts = ghostObject.GetComponentsInChildren<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts) {
            script.enabled = false;
        }
    }

    void updateGhostColor(Color color) {
        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer render in renderers) {
            Material material = render.material;
            material.color = color;
        }
    }

    public Vector3 getPlayerSpawnPosition() {
        return playerStartPosition;
    }

    public Quaternion getPlayerSpawnRotation() {
        return playerStartRotation;
    }

    public void setLevelComplete(bool complete) {
        levelComplete = complete;
        if (saveLevelUI == null)
        {
            return;
        }
        if (complete) {
            saveLevelUI.OpenSaveUI();
        } else {
            saveLevelUI.CloseSaveUI();
        }
    }

    public void RespawnPlayer() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            player.transform.position = playerStartPosition;
            player.transform.rotation = playerStartRotation;
        }
    }

    public void StartMovement() {
        Transform character = placementContainer.transform.Find("Character(Clone)");
        CharacterMovement movementScript = character.GetComponent<CharacterMovement>();
        movementScript.StartMovement();
    }
}
