using UnityEngine;

public class SettingsToggle : MonoBehaviour
{
    public SettingsManager settingsManager;
    public Canvas settingsCanvas;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsCanvas.isActiveAndEnabled)
            {
                settingsManager.CloseSettings();
            }
            else
            {
                settingsManager.OpenSettings();
            }
        }
    }
}