using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutonomousCharacter : MonoBehaviour
{

    public GridManager gridManager;
    public Vector3[] path;
    private int currentPathIndex = 0;
    private Vector3 targetPosition;
    public Vector3Int startPosition = new Vector3Int(0, 0, 0);
    public float speed;
    //public Vector3 moveDirection = new Vector3(1, 0, 0);
    
    void Start() {
        if (gridManager == null) {
            gridManager = Object.FindFirstObjectByType<GridManager>();
        }

        Vector3 spawnPosition = gridManager.GridToWorld(startPosition);
        spawnPosition.y += 0.5f; // Adjust for character height
        transform.position = spawnPosition;

        // Define a simple path
        path = new Vector3[] {
            new Vector3(0, 0, 0),
            new Vector3(3, 0, 0),
            new Vector3(3, 0, 3),
            new Vector3(0, 0, 3),
            new Vector3(0, 0, 0)
        };

        targetPosition = gridManager.GridToWorld(Vector3Int.RoundToInt(path[currentPathIndex]));
    }

    // Update is called once per frame
    void Update()
    {
        //if (path.Length - 1 == currentPathIndex) return;
        //GetComponent<Rigidbody>().AddForce(getMoveDirection() * speed * Time.deltaTime);
        Vector3 moveDirection = getMoveDirection();
        Vector3 move = moveDirection * speed * Time.deltaTime;
        GetComponent<Rigidbody>().MovePosition(transform.position + move);
    }

    void setNextPathPoint() {
        currentPathIndex++;
        if (currentPathIndex < path.Length) {
            targetPosition = gridManager.GridToWorld(Vector3Int.RoundToInt(path[currentPathIndex]));
            targetPosition.y += 0.5f; // Adjust for character height
        }
    }

    Vector3 getMoveDirection() {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        if (direction.magnitude < 0.05f) {
            setNextPathPoint();
            direction = targetPosition - transform.position;
        }
        return direction.normalized;
    }
}
