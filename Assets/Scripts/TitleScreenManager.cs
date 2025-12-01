using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    // This function will be called when the Start button is pressed
    public void StartGame()
    {
        // Replace "Level1" with the name of your gameplay scene
        SceneManager.LoadScene("SampleScene");
    }

    // This function will be called when the Quit button is pressed
    public void QuitGame()
    {
        // This will close the game build
        Application.Quit();

        // This makes the Quit button work in the Unity editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
