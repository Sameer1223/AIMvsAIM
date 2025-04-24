using UnityEngine;
using UnityEngine.UI;  // Make sure to import this to use Button

public class QuitScript : MonoBehaviour
{
    public Button quitButton;  // Drag your Quit Button here

    private void Start()
    {
        quitButton.onClick.AddListener(QuitGame);
    }

    private void QuitGame()
    {
        // This only works in builds
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;  // If in the editor, stop play mode
#else
            Application.Quit();  // Close the application in a build
#endif
    }
}