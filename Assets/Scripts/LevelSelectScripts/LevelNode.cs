using UnityEngine;
using UnityEngine.UI;

public class LevelNode : MonoBehaviour
{
    public int levelIndex;
    public GameObject lockIcon;
    public Button levelButton;
    public GameObject UnlockedIcon;


    void Start()
    {
        bool unlocked = levelIndex <= ProgressManager.HighestUnlockedLevel;

        levelButton.interactable = unlocked;
        if (lockIcon != null)
            lockIcon.SetActive(!unlocked);
    }
}