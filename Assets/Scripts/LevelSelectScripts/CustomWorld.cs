using UnityEngine;

public class CustomWorld : MonoBehaviour
{
    public LockedPopup lockedPopup;
    public GameObject customWorldPanel;

    public void OnClick()
    {
// If all main levels are not completed, show warning
        if (!ProgressManager.AllLevelsCompleted())
        {
            if (lockedPopup != null)
                lockedPopup.Show();

            return;
        }

//open custom world panel when all levels completed
        if (customWorldPanel != null)
            customWorldPanel.SetActive(true);
    }
}
