using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            GridManager gridManager = FindAnyObjectByType<GridManager>();
            if (gridManager != null)
            {
                gridManager.TriggerCheckpoint(transform.position);
            }
        }
    }
}
