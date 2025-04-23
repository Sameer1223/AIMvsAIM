using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public Image crosshairImage;
    public Sprite[] crosshairSprites; // Set these in the inspector
    public Color[] crosshairColors;   // Define a few preset colors
    
    private void Start()
    {
        ApplyCrosshairSettings();
    }

    public void ApplyCrosshairSettings()
    {
        int typeIndex = PlayerPrefs.GetInt("CrosshairType", 0);
        float size = PlayerPrefs.GetFloat("CrosshairSize", 2.5f);
        int colorIndex = PlayerPrefs.GetInt("CrosshairColor", 0);

        // Change sprite
        if (typeIndex >= 0 && typeIndex < crosshairSprites.Length)
        {
            crosshairImage.sprite = crosshairSprites[typeIndex];
        }

        // Change size
        crosshairImage.rectTransform.localScale = Vector3.one * size;

        // Change color
        if (colorIndex >= 0 && colorIndex < crosshairColors.Length)
        {
            crosshairImage.color = crosshairColors[colorIndex];
        }
    }
}