using UnityEngine;
using System.Collections;

public class HoldPlayer : MonoBehaviour
{
    public bool canMove = true;
// Controls whether the player can move or not

    void Update()
    {
// If movement is disabled, stop here
        if (!canMove)
            return;
    }

// Disable player movement, wait for a spesific time and move again
    public IEnumerator HoldMovement(float duration)
    {
        canMove = false;
        yield return new WaitForSeconds(duration);
        canMove = true;
    }
}