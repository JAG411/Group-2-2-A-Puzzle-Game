using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Vector3 cellSize = Vector3.one;
    public Camera mainCamera;
    public bool placement = false;
    public GameObject prefabToPlace;
    public Transform placementContainer;
    private GameObject ghostObject;

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

            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                if (hit.collider.gameObject.tag != "Ground") {
                    updateGhostColor(Color.red);
                } else {
                    updateGhostColor(Color.green);
                }
                Vector3 worldPos = hit.point;
                Vector3Int gridPos = WorldToGrid(worldPos);
                Vector3 snappedWorldPos = GridToWorld(gridPos);
                snappedWorldPos.y = 0.5f; // Adjust for object height

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
                    snappedWorldPos.y = 0.5f; // Adjust for object height
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
        }
    }

    public void setPrefabPlacement(GameObject prefab) {
        prefabToPlace = prefab;
        placement = true;
        CreateGhostObject();
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
            color.a = 0.9f;
            material.color = color;
            material.renderQueue = 3000; // Ensure it's rendered on top
        }
    }

    void updateGhostColor(Color color) {
        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer render in renderers) {
            Material material = render.material;
            material.color = color;
        }
    }
}
