using UnityEngine;

public class Switch : MonoBehaviour
{
    public Door connectedToDoor;
    public Vector2Int moveDirection;

    private bool isPressed = false;

// Opens the related door once when the player presses on it
    public void SetPressed()
    {
        if (isPressed) return;
        isPressed = true;
        if (connectedToDoor != null)
        {
            connectedToDoor.SetOpen(true);
        }
    }
}