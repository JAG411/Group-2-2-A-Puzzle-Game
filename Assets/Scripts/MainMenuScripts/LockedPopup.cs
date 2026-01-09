using UnityEngine;

public class LockedPopup : MonoBehaviour
{
    public GameObject popupRoot;

    void Start()
    {
        popupRoot.SetActive(false);
    }

    public void Show()
    {
        popupRoot.SetActive(true);
    }

    public void Close()
    {
        popupRoot.SetActive(false);
    }
}
