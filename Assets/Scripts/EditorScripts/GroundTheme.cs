using UnityEngine;

public class GroundTheme : MonoBehaviour
{
    public MeshRenderer groundRenderer;
    public int currentThemeIndex = 0;

// Fixed list of ground materials (5 themes)
    public Material[] groundThemes;

// Change ground theme by index which is called from UI buttons
    public void SetTheme(int index)
    {
        if (groundRenderer == null) return;
        if (index < 0 || index >= groundThemes.Length) return;
        currentThemeIndex = index;
        groundRenderer.material = groundThemes[index];
    }
}
