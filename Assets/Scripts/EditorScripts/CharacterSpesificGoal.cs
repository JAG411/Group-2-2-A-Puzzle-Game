using UnityEngine;

public class CharacterSpesificGoal : MonoBehaviour
{
    public string expectedTag = "character2";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(expectedTag))
        {
            GridManager gridManager = FindAnyObjectByType<GridManager>();
            if (gridManager != null)
            {
                gridManager.RegisterGoalReached(expectedTag);
            }
        }
    }
}