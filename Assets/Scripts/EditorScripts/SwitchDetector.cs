using UnityEngine;

public class SwitchDetector : MonoBehaviour
{
    private GridManager gridManager;

    void Start()
    {
//Find the GridManager in the scene
        gridManager = Object.FindFirstObjectByType<GridManager>();
    }

    void Update()
    {
//Get the current position of the player
        Vector3 characterPos = transform.position;

//Check if the player is standing on a switch tile
        var switchTile = gridManager.GetSwitchAtWorld(characterPos);

//If there is a switch under the player, press it
        if (switchTile != null)
        {
            switchTile.SetPressed(); // one-time press
        }
    }
}