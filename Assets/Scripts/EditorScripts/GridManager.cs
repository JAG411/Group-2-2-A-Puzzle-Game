using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GridManager : MonoBehaviour
{
    public Vector3 cellSize = Vector3.one;
    public Camera mainCamera;
    public bool placement = false;
    public GameObject prefabToPlace;
    private Quaternion prefabRotation;
    public Transform placementContainer;
    private GameObject ghostObject;
    private float offsetY = 0.5f;
    public PrefabOptionsMenu prefabOptionsMenu;
    public bool UI = false;
    public SaveLoadManager saveLoadManager;
    private Vector3 playerStartPosition;
    private Quaternion playerStartRotation;
    private Vector3 groundPosition;
    private Vector3 groundScale;
    private Vector3 northArrowsPosition;
    private Vector3 southArrowsPosition;
    private Vector3 eastArrowsPosition;
    private Vector3 westArrowsPosition;
    public bool levelComplete = false;
    public SaveLevelUI saveLevelUI;
    public CameraMovement mainCameraMovement;

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


    void Update()
    {
        if (placement) {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Input.GetKeyDown(KeyCode.R)) {
                ghostObject.transform.Rotate(0, 90, 0);
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
                        playerStartRotation = ghostObject.transform.rotation;
                    }
                    setLevelComplete(false);
                    Instantiate(prefabToPlace, snappedWorldPos, ghostObject.transform.rotation, placementContainer);
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
                    if (hit.collider.gameObject.tag == "Rotatable" || hit.collider.gameObject.tag == "Direction" || hit.collider.gameObject.tag == "Goal" || ((hit.collider.gameObject.tag == "Player" || hit.collider.gameObject.tag == "Enemy") && currentScene.name == "LevelEditor")) {
                        UI = true;
                        prefabOptionsMenu.OpenMenu(hit.collider.gameObject);
                    } else if (hit.collider.gameObject.tag == "Expand") {
                        GameObject parentObject = hit.collider.gameObject.transform.parent.gameObject;
                        if (parentObject.name.Contains("North")) {
                            expandGround("North", parentObject);
                        } else if (parentObject.name.Contains("South")) {
                            expandGround("South", parentObject);
                        } else if (parentObject.name.Contains("East")) {
                            expandGround("East", parentObject);
                        } else if (parentObject.name.Contains("West")) {
                            expandGround("West", parentObject);

                        }
                    } else if (hit.collider.gameObject.tag == "Contract") {
                        GameObject parentObject = hit.collider.gameObject.transform.parent.gameObject;
                        if (parentObject.name.Contains("North")) {
                            contractGround("North", parentObject);
                        } else if (parentObject.name.Contains("South")) {
                            contractGround("South", parentObject);
                        } else if (parentObject.name.Contains("East")) {
                            contractGround("East", parentObject);
                        } else if (parentObject.name.Contains("West")) {
                            contractGround("West", parentObject);
                        }
                    }
                }
            }
        } else if (UI) {
            if (Input.GetMouseButtonDown(1) || (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())) {
                UI = false;
                prefabOptionsMenu.CloseMenu();
            }
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
            mainCameraMovement.StopMovement();
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

    public void BackToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void expandGround(string direction, GameObject parent) {
        if (direction == "North") {
            transform.localScale += new Vector3(0, 0, 0.1f);
            transform.position += new Vector3(0, 0, 0.5f);
            adjustArrowPositions(true, "North");
        } else if (direction == "South") {
            transform.localScale += new Vector3(0, 0, 0.1f);
            transform.position += new Vector3(0, 0, -0.5f);
            adjustArrowPositions(true, "South");
        } else if (direction == "East") {
            transform.localScale += new Vector3(0.1f, 0, 0);
            transform.position += new Vector3(0.5f, 0, 0);
            adjustArrowPositions(true, "East");
        } else if (direction == "West") {
            transform.localScale += new Vector3(0.1f, 0, 0);
            transform.position += new Vector3(-0.5f, 0, 0);
            adjustArrowPositions(true, "West");
        }
        groundPosition = transform.position;
        groundScale = transform.localScale;
    }

    public void contractGround(string direction, GameObject parent) {
        if (direction == "North") {
            // Prevent contracting below minimum size
            if (transform.localScale.z <= 1.1f) {
                Debug.Log("Cannot contract further North");
                return;
            }
            transform.localScale += new Vector3(0, 0, -0.1f);
            transform.position += new Vector3(0, 0, -0.5f);
            adjustArrowPositions(false, "North");
        } else if (direction == "South") {
            if (transform.localScale.z <= 1.1f) {
                Debug.Log("Cannot contract further North");
                return;
            }
            transform.localScale += new Vector3(0, 0, -0.1f);
            transform.position += new Vector3(0, 0, 0.5f);
            adjustArrowPositions(false, "South");
        } else if (direction == "East") {
            if (transform.localScale.x <= 1.1f) {
                Debug.Log("Cannot contract further East");
                return;
            }
            transform.localScale += new Vector3(-0.1f, 0, 0);
            transform.position += new Vector3(-0.5f, 0, 0);
            adjustArrowPositions(false, "East");
        } else if (direction == "West") {
            if (transform.localScale.x <= 1.1f) {
                Debug.Log("Cannot contract further West");
                return;
            }
            transform.localScale += new Vector3(-0.1f, 0, 0);
            transform.position += new Vector3(0.5f, 0, 0);
            adjustArrowPositions(false, "West");
        }
        groundPosition = transform.position;
        groundScale = transform.localScale;
    }

    public void adjustArrowPositions(bool expand, string direction) {
        // Adjust positions of arrows based on ground size changes
        GameObject northArrows = GameObject.Find("Arrows_North");
        GameObject southArrows = GameObject.Find("Arrows_South");
        GameObject eastArrows = GameObject.Find("Arrows_East");
        GameObject westArrows = GameObject.Find("Arrows_West");
        float adjustmentAmount = 1f;

        if (expand) {
            if (direction == "North") {
                northArrows.transform.position += new Vector3(0, 0, adjustmentAmount);
                westArrows.transform.position += new Vector3(0, 0, adjustmentAmount/2);
                eastArrows.transform.position += new Vector3(0, 0, adjustmentAmount/2);
            } else if (direction == "South") {
                southArrows.transform.position += new Vector3(0, 0, -adjustmentAmount);
                westArrows.transform.position += new Vector3(0, 0, (-adjustmentAmount)/2);
                eastArrows.transform.position += new Vector3(0, 0, (-adjustmentAmount)/2);
            } else if (direction == "East") {
                eastArrows.transform.position += new Vector3(adjustmentAmount, 0, 0);
                northArrows.transform.position += new Vector3(adjustmentAmount/2, 0, 0);
                southArrows.transform.position += new Vector3(adjustmentAmount/2, 0, 0);
            } else if (direction == "West") {
                westArrows.transform.position += new Vector3(-adjustmentAmount, 0, 0);
                northArrows.transform.position += new Vector3((-adjustmentAmount)/2, 0, 0);
                southArrows.transform.position += new Vector3((-adjustmentAmount)/2, 0, 0);
            }
        } else {
            if (direction == "North") {
                northArrows.transform.position += new Vector3(0, 0, -adjustmentAmount);
                westArrows.transform.position += new Vector3(0, 0, (-adjustmentAmount)/2);
                eastArrows.transform.position += new Vector3(0, 0, (-adjustmentAmount)/2);
            } else if (direction == "South") {
                southArrows.transform.position += new Vector3(0, 0, adjustmentAmount);
                westArrows.transform.position += new Vector3(0, 0, adjustmentAmount/2);
                eastArrows.transform.position += new Vector3(0, 0, adjustmentAmount/2);
            } else if (direction == "East") {
                eastArrows.transform.position += new Vector3(-adjustmentAmount, 0, 0);
                northArrows.transform.position += new Vector3((-adjustmentAmount)/2, 0, 0);
                southArrows.transform.position += new Vector3((-adjustmentAmount)/2, 0, 0);
            } else if (direction == "West") {
                westArrows.transform.position += new Vector3(adjustmentAmount, 0, 0);
                northArrows.transform.position += new Vector3(adjustmentAmount/2, 0, 0);
                southArrows.transform.position += new Vector3(adjustmentAmount/2, 0, 0);
            }
        }
        northArrowsPosition = northArrows.transform.position;
        southArrowsPosition = southArrows.transform.position;
        eastArrowsPosition = eastArrows.transform.position;
        westArrowsPosition = westArrows.transform.position;
    }

    private bool IsPointerOverUIObject() {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public Vector3 getGroundPosition() {
        return groundPosition;
    }

    public Vector3 getGroundScale() {
        return groundScale;
    }

    public Vector3 getNorthArrowsPosition() {
        return northArrowsPosition;
    }

    public Vector3 getSouthArrowsPosition() {
        return southArrowsPosition;
    }

    public Vector3 getEastArrowsPosition() {
        return eastArrowsPosition;
    }

    public Vector3 getWestArrowsPosition() {
        return westArrowsPosition;
    }
}
