using UnityEngine;

public class Door : MonoBehaviour
{
// Stores if the switch was pressed
    private bool isOpen = false;

// Called by the switch
    public void SetOpen(bool open)
    {
        isOpen = open;
    }

// Used by CollisionBehaviour
    public bool IsOpen()
    {
        return isOpen;
    }
}